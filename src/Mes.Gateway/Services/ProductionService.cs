namespace Mes.Gateway.Services;

public class ProductionService
{
    private readonly QualityService _qualityService;

    private int _targetQty = 60;


    public ProductionService(
        QualityService qualityService)
    {
        _qualityService = qualityService;
    }


    public int TargetQty
    {
        get => _targetQty;

        set
        {
            if (value <= 0)
                throw new ArgumentException(
                    "목표 수량은 1 이상이어야 합니다."
                );

            _targetQty = value;
        }
    }


    public int ProducedQty =>
        _qualityService.TotalCount;


    public int GoodQty =>
        _qualityService.PassCount;


    public int DefectQty =>
        _qualityService.FailCount;


    public double AchievementRate
    {
        get
        {
            if (TargetQty == 0)
                return 0;

            return
                (double)ProducedQty
                / TargetQty
                * 100.0;
        }
    }


    public double YieldRate =>
        _qualityService.YieldRate;


    public void PrintStatus()
    {
        Console.WriteLine();
        Console.WriteLine(
            "========== PRODUCTION STATUS =========="
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

        Console.WriteLine(
            "======================================="
        );
    }
}