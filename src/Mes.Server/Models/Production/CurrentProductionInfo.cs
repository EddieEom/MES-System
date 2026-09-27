namespace Mes.Server.Models.Production;

public class CurrentProductionInfo
{
    public WorkOrderProductionInfo WorkOrder { get; set; } =
        new();

    public LotProductionInfo? CurrentLot { get; set; }
}


public class WorkOrderProductionInfo
{
    public long WorkOrderId { get; set; }

    public string WorkOrderCode { get; set; } =
        string.Empty;

    public string Status { get; set; } =
        string.Empty;

    public int TargetQty { get; set; }

    public int ProducedQty { get; set; }

    public int GoodQty { get; set; }

    public int DefectQty { get; set; }


    public double AchievementRate =>
        TargetQty == 0
            ? 0
            : (double)ProducedQty
                / TargetQty
                * 100.0;


    public double YieldRate =>
        ProducedQty == 0
            ? 0
            : (double)GoodQty
                / ProducedQty
                * 100.0;
}


public class LotProductionInfo
{
    public long LotId { get; set; }

    public string LotCode { get; set; } =
        string.Empty;

    public short LotSequence { get; set; }

    public string Status { get; set; } =
        string.Empty;

    public int TargetQty { get; set; }

    public int ProducedQty { get; set; }

    public int GoodQty { get; set; }

    public int DefectQty { get; set; }


    public double ProgressRate =>
        TargetQty == 0
            ? 0
            : (double)ProducedQty
                / TargetQty
                * 100.0;


    public double YieldRate =>
        ProducedQty == 0
            ? 0
            : (double)GoodQty
                / ProducedQty
                * 100.0;
}