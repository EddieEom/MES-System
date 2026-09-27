using Mes.Gateway.Api;
using Mes.Gateway.Models.Alarm;

namespace Mes.Gateway.Services;

public class AlarmService
{
    private readonly IMesApiClient _mesApiClient;

    private readonly Dictionary<
        string,
        AlarmApiResponse
    > _activeAlarms =
        new(
            StringComparer.OrdinalIgnoreCase
        );


    private readonly SemaphoreSlim _lock =
        new(1, 1);


    public AlarmService(
        IMesApiClient mesApiClient)
    {
        _mesApiClient =
            mesApiClient;
    }


    // ======================================
    // Gateway 시작 시
    // Server ACTIVE Alarm 복구
    // ======================================

    public async Task<bool> SyncActiveAsync(
        CancellationToken cancellationToken = default)
    {
        var alarms =
            await _mesApiClient
                .GetActiveAlarmsAsync(
                    cancellationToken
                );


        if (alarms == null)
        {
            Console.WriteLine(
                "[ALARM SERVICE] ACTIVE Alarm 동기화 실패"
            );

            return false;
        }


        await _lock.WaitAsync(
            cancellationToken
        );


        try
        {
            _activeAlarms.Clear();


            foreach (var alarm in alarms)
            {
                if (!alarm.IsActive)
                    continue;


                _activeAlarms[
                    CreateKey(
                        alarm.MachineCode,
                        alarm.AlarmCode
                    )
                ] = alarm;
            }
        }
        finally
        {
            _lock.Release();
        }


        Console.WriteLine(
            $"[ALARM SERVICE] ACTIVE Alarm 동기화 완료 - {_activeAlarms.Count}건"
        );


        return true;
    }


    // ======================================
    // Alarm 발생
    // ======================================

    public async Task<bool> RaiseAsync(
        string machineCode,
        string alarmCode,
        string? alarmMessage,
        string severity = "WARNING",
        DateTimeOffset? occurredAt = null,
        string sourceSystem = "PLC",
        CancellationToken cancellationToken = default)
    {
        string key =
            CreateKey(
                machineCode,
                alarmCode
            );


        await _lock.WaitAsync(
            cancellationToken
        );


        try
        {
            // 이미 같은 설비/코드 Alarm이 ACTIVE라면
            // 중복 INSERT하지 않음.
            if (_activeAlarms.ContainsKey(
                key))
            {
                Console.WriteLine(
                    $"[ALARM SERVICE] 이미 ACTIVE - {machineCode}/{alarmCode}"
                );

                return true;
            }


            var request =
                new AlarmCreateApiRequest
                {
                    MachineCode =
                        machineCode,

                    AlarmCode =
                        alarmCode,

                    AlarmMessage =
                        alarmMessage,

                    Severity =
                        severity,

                    OccurredAt =
                        occurredAt
                        ?? DateTimeOffset.UtcNow,

                    SourceSystem =
                        sourceSystem
                };


            var alarm =
                await _mesApiClient
                    .PostAlarmAsync(
                        request,
                        cancellationToken
                    );


            if (alarm == null)
            {
                return false;
            }


            _activeAlarms[key] =
                alarm;


            Console.WriteLine(
                $"[ALARM SERVICE] ACTIVE - {machineCode}/{alarmCode} / ID={alarm.AlarmId}"
            );


            return true;
        }
        finally
        {
            _lock.Release();
        }
    }


    // ======================================
    // Alarm 해제
    //
    // PLC에서는 AlarmId가 아니라
    // MachineCode + AlarmCode를 알고 있으므로
    // 내부에서 AlarmId를 찾아 Clear
    // ======================================

    public async Task<bool> ClearAsync(
        string machineCode,
        string alarmCode,
        CancellationToken cancellationToken = default)
    {
        string key =
            CreateKey(
                machineCode,
                alarmCode
            );


        await _lock.WaitAsync(
            cancellationToken
        );


        try
        {
            if (!_activeAlarms.TryGetValue(
                key,
                out var alarm))
            {
                Console.WriteLine(
                    $"[ALARM SERVICE] Clear 대상 ACTIVE Alarm 없음 - {machineCode}/{alarmCode}"
                );

                return true;
            }


            bool cleared =
                await _mesApiClient
                    .ClearAlarmAsync(
                        alarm.AlarmId,
                        cancellationToken
                    );


            if (!cleared)
            {
                return false;
            }


            _activeAlarms.Remove(
                key
            );


            Console.WriteLine(
                $"[ALARM SERVICE] CLEARED - {machineCode}/{alarmCode} / ID={alarm.AlarmId}"
            );


            return true;
        }
        finally
        {
            _lock.Release();
        }
    }


    // ======================================
    // 현재 Gateway가 알고 있는 ACTIVE Alarm
    // ======================================

    public async Task<IReadOnlyList<AlarmApiResponse>>
        GetActiveSnapshotAsync(
            CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(
            cancellationToken
        );


        try
        {
            return _activeAlarms
                .Values
                .ToList();
        }
        finally
        {
            _lock.Release();
        }
    }


    private static string CreateKey(
        string machineCode,
        string alarmCode)
    {
        return
            $"{machineCode.Trim()}|{alarmCode.Trim()}";
    }
}