using System.Collections.Concurrent;
using Mes.Gateway.Models.Quailty;

namespace Mes.Gateway.Services;

public class QualityService
{
    private readonly ConcurrentDictionary<string, byte>
        _processedEventIds = new();

    private int _passCount;
    private int _failCount;


    public bool Process(QualityResultMessage message)
    {
        // EventId가 없으면 정상 이벤트로 처리하지 않음
        if (string.IsNullOrWhiteSpace(message.EventId))
        {
            Console.WriteLine(
                "[QUALITY SERVICE] EventId 없음"
            );

            return false;
        }


        // 이미 처리한 EventId이면 중복 처리 방지
        if (!_processedEventIds.TryAdd(
                message.EventId,
                0))
        {
            Console.WriteLine(
                $"[QUALITY SERVICE] 중복 이벤트 무시: {message.EventId}"
            );

            return false;
        }


        switch (message.Result.ToUpperInvariant())
        {
            case "PASS":

                Interlocked.Increment(
                    ref _passCount
                );

                break;


            case "FAIL":

                Interlocked.Increment(
                    ref _failCount
                );

                break;


            default:

                Console.WriteLine(
                    $"[QUALITY SERVICE] 알 수 없는 판정: {message.Result}"
                );

                return false;
        }


        Console.WriteLine(
            $"[QUALITY SERVICE] {message.Result} 집계 완료"
        );

        Console.WriteLine(
            $"PASS={PassCount}, FAIL={FailCount}, TOTAL={TotalCount}, YIELD={YieldRate:F2}%"
        );

        return true;
    }


    public int PassCount =>
        Volatile.Read(ref _passCount);


    public int FailCount =>
        Volatile.Read(ref _failCount);


    public int TotalCount =>
        PassCount + FailCount;


    public double YieldRate
    {
        get
        {
            if (TotalCount == 0)
                return 0;

            return
                (double)PassCount
                / TotalCount
                * 100.0;
        }
    }
}