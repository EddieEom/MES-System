using Mes.Gateway.Api;
using Mes.Gateway.Models.Production;

namespace Mes.Gateway.Services;

public class ProductionService
{
    private readonly IMesApiClient _mesApiClient;

    private CurrentProductionApiResponse? _current;


    public ProductionService(
        IMesApiClient mesApiClient)
    {
        _mesApiClient =
            mesApiClient;
    }


    // ======================================
    // 현재 DB 생산실적 Snapshot
    // ======================================

    public CurrentProductionApiResponse? Current =>
        _current;


    public bool HasData =>
        _current != null;


    // ======================================
    // WorkOrder
    // ======================================

    public string WorkOrderCode =>
        _current?.WorkOrder.WorkOrderCode
        ?? string.Empty;


    public string WorkOrderStatus =>
        _current?.WorkOrder.Status
        ?? string.Empty;


    public int TargetQty =>
        _current?.WorkOrder.TargetQty
        ?? 0;


    public int ProducedQty =>
        _current?.WorkOrder.ProducedQty
        ?? 0;


    public int GoodQty =>
        _current?.WorkOrder.GoodQty
        ?? 0;


    public int DefectQty =>
        _current?.WorkOrder.DefectQty
        ?? 0;


    public double AchievementRate =>
        _current?.WorkOrder.AchievementRate
        ?? 0;


    public double YieldRate =>
        _current?.WorkOrder.YieldRate
        ?? 0;


    // ======================================
    // Current LOT
    // ======================================

    public string CurrentLotCode =>
        _current?.CurrentLot?.LotCode
        ?? string.Empty;


    public string CurrentLotStatus =>
        _current?.CurrentLot?.Status
        ?? string.Empty;


    public int CurrentLotTargetQty =>
        _current?.CurrentLot?.TargetQty
        ?? 0;


    public int CurrentLotProducedQty =>
        _current?.CurrentLot?.ProducedQty
        ?? 0;


    public int CurrentLotGoodQty =>
        _current?.CurrentLot?.GoodQty
        ?? 0;


    public int CurrentLotDefectQty =>
        _current?.CurrentLot?.DefectQty
        ?? 0;


    public double CurrentLotProgressRate =>
        _current?.CurrentLot?.ProgressRate
        ?? 0;


    public double CurrentLotYieldRate =>
        _current?.CurrentLot?.YieldRate
        ?? 0;


    // ======================================
    // Mes.Server → DB 생산실적 갱신
    // ======================================

    public async Task<bool> RefreshAsync(
        CancellationToken cancellationToken = default)
    {
        var latest =
            await _mesApiClient
                .GetCurrentProductionAsync(
                    cancellationToken
                );


        if (latest == null)
        {
            Console.WriteLine(
                "[PRODUCTION SERVICE] DB 생산실적을 가져오지 못했습니다."
            );

            // 기존 Snapshot이 있다면
            // 일시적인 HTTP 장애 때문에 지우지 않음
            return false;
        }


        _current =
            latest;


        Console.WriteLine(
            $"[PRODUCTION SERVICE] DB 생산실적 갱신 - {WorkOrderCode}"
        );

        Console.WriteLine(
            $"[PRODUCTION SERVICE] Produced={ProducedQty}, Good={GoodQty}, Defect={DefectQty}"
        );


        return true;
    }


    // ======================================
    // Console Production Status
    // ======================================

    public void PrintStatus()
    {
        Console.WriteLine();

        Console.WriteLine(
            "========== PRODUCTION STATUS =========="
        );


        if (_current == null)
        {
            Console.WriteLine(
                "DB 생산실적 데이터 없음"
            );

            Console.WriteLine(
                "======================================="
            );

            return;
        }


        Console.WriteLine(
            $"WorkOrder   : {WorkOrderCode}"
        );

        Console.WriteLine(
            $"Status      : {WorkOrderStatus}"
        );

        Console.WriteLine(
            $"Target      : {TargetQty}"
        );

        Console.WriteLine(
            $"Produced    : {ProducedQty}"
        );

        Console.WriteLine(
            $"Good        : {GoodQty}"
        );

        Console.WriteLine(
            $"Defect      : {DefectQty}"
        );

        Console.WriteLine(
            $"Achievement : {AchievementRate:F2}%"
        );

        Console.WriteLine(
            $"Yield       : {YieldRate:F2}%"
        );


        Console.WriteLine();


        if (_current.CurrentLot != null)
        {
            Console.WriteLine(
                $"Current LOT : {CurrentLotCode}"
            );

            Console.WriteLine(
                $"LOT Status  : {CurrentLotStatus}"
            );

            Console.WriteLine(
                $"LOT Target  : {CurrentLotTargetQty}"
            );

            Console.WriteLine(
                $"LOT Produced: {CurrentLotProducedQty}"
            );

            Console.WriteLine(
                $"LOT Good    : {CurrentLotGoodQty}"
            );

            Console.WriteLine(
                $"LOT Defect  : {CurrentLotDefectQty}"
            );

            Console.WriteLine(
                $"LOT Progress: {CurrentLotProgressRate:F2}%"
            );

            Console.WriteLine(
                $"LOT Yield   : {CurrentLotYieldRate:F2}%"
            );
        }


        Console.WriteLine(
            "======================================="
        );
    }
}