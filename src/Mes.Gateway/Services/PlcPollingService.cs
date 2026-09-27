using Mes.Gateway.Plc;

namespace Mes.Gateway.Services;

public sealed class PlcPollingService
{
    private readonly IPlcClient _plcClient;
    private readonly AlarmService _alarmService;

    private readonly string _machineCode;

    private int? _lastStateCode;
    private int? _lastAlarmCode;


    public PlcSnapshot? Current { get; private set; }


    public PlcPollingService(
        IPlcClient plcClient,
        AlarmService alarmService,
        string machineCode)
    {
        _plcClient =
            plcClient;

        _alarmService =
            alarmService;

        _machineCode =
            machineCode;
    }


    // ======================================
    // 1초 주기 PLC Polling
    // ======================================

    public async Task RunAsync(
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(
            "[PLC POLLING] 시작 - 1초 주기"
        );


        try
        {
            while (!cancellationToken
                .IsCancellationRequested)
            {
                await ReadAndProcessAsync(
                    cancellationToken
                );


                await Task.Delay(
                    1000,
                    cancellationToken
                );
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken
                .IsCancellationRequested)
        {
            // 정상 종료
        }


        Console.WriteLine(
            "[PLC POLLING] 종료"
        );
    }


    // ======================================
    // PLC 상태 읽기 + 처리
    // ======================================

    private async Task ReadAndProcessAsync(
        CancellationToken cancellationToken)
    {
        int? stateCode =
            await _plcClient
                .ReadDeviceAsync(
                    "D110",
                    cancellationToken
                );


        int? alarmCode =
            await _plcClient
                .ReadDeviceAsync(
                    "D111",
                    cancellationToken
                );


        if (stateCode == null ||
            alarmCode == null)
        {
            Console.WriteLine(
                "[PLC POLLING] PLC 상태 읽기 실패"
            );

            return;
        }


        var snapshot =
            new PlcSnapshot
            {
                StateCode =
                    stateCode.Value,

                AlarmCode =
                    alarmCode.Value,

                ReadAt =
                    DateTimeOffset.UtcNow
            };


        Current =
            snapshot;


        // 상태 또는 AlarmCode가 바뀐 경우만 로그
        if (_lastStateCode !=
                snapshot.StateCode ||
            _lastAlarmCode !=
                snapshot.AlarmCode)
        {
            Console.WriteLine(
                $"[PLC POLLING] State={snapshot.State}({snapshot.StateCode}) / AlarmCode={snapshot.AlarmCode}"
            );
        }


        await SynchronizeAlarmAsync(
            snapshot.AlarmCode,
            cancellationToken
        );


        _lastStateCode =
            snapshot.StateCode;

        _lastAlarmCode =
            snapshot.AlarmCode;
    }


    // ======================================
    // PLC Alarm ↔ MES Alarm 동기화
    //
    // D111 = 0
    // → PLC Alarm 없음
    //
    // D111 > 0
    // → PLC-{code} Alarm ACTIVE
    // ======================================

    private async Task SynchronizeAlarmAsync(
        int alarmCode,
        CancellationToken cancellationToken)
    {
        var activeAlarms =
            await _alarmService
                .GetActiveSnapshotAsync(
                    cancellationToken
                );


        var plcAlarms =
            activeAlarms
                .Where(
                    x =>
                        string.Equals(
                            x.MachineCode,
                            _machineCode,
                            StringComparison.OrdinalIgnoreCase
                        )
                        &&
                        string.Equals(
                            x.SourceSystem,
                            "PLC",
                            StringComparison.OrdinalIgnoreCase
                        )
                        &&
                        x.AlarmCode.StartsWith(
                            "PLC-",
                            StringComparison.OrdinalIgnoreCase
                        )
                )
                .ToList();


        // ======================================
        // 정상 상태
        // ======================================

        if (alarmCode == 0)
        {
            foreach (var alarm in plcAlarms)
            {
                await _alarmService
                    .ClearAsync(
                        alarm.MachineCode,
                        alarm.AlarmCode,
                        cancellationToken
                    );
            }


            return;
        }


        string currentAlarmCode =
            $"PLC-{alarmCode:D3}";


        // ======================================
        // 다른 PLC Alarm이 ACTIVE라면 먼저 Clear
        // ======================================

        foreach (
            var alarm
            in plcAlarms.Where(
                x =>
                    !string.Equals(
                        x.AlarmCode,
                        currentAlarmCode,
                        StringComparison.OrdinalIgnoreCase
                    )
            ))
        {
            await _alarmService
                .ClearAsync(
                    alarm.MachineCode,
                    alarm.AlarmCode,
                    cancellationToken
                );
        }


        // ======================================
        // 현재 Alarm이 이미 ACTIVE면 종료
        // ======================================

        if (plcAlarms.Any(
            x =>
                string.Equals(
                    x.AlarmCode,
                    currentAlarmCode,
                    StringComparison.OrdinalIgnoreCase
                )))
        {
            return;
        }


        // ======================================
        // 신규 Alarm ACTIVE
        // ======================================

        await _alarmService
            .RaiseAsync(
                machineCode:
                    _machineCode,

                alarmCode:
                    currentAlarmCode,

                alarmMessage:
                    $"PLC Alarm Code {alarmCode}",

                severity:
                    "WARNING",

                occurredAt:
                    DateTimeOffset.UtcNow,

                sourceSystem:
                    "PLC",

                cancellationToken:
                    cancellationToken
            );
    }
}