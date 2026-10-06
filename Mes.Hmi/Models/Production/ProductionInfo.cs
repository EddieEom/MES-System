namespace Mes.Hmi.Models.Production;

public class ProductionInfo
{
    public WorkOrderInfo? WorkOrder { get; set; }
    public LotInfo? CurrentLot { get; set; }
}

public class WorkOrderInfo
{
    public int WorkOrderId { get; set; }
    public string? WorkOrderCode { get; set; }
    public string? Status { get; set; }

    public int TargetQty { get; set; }
    public int ProducedQty { get; set; }
    public int GoodQty { get; set; }
    public int DefectQty { get; set; }
    public double AchievementRate { get; set; }
    public double YieldRate { get; set; }
}

public class LotInfo
{
    public int LotId { get; set; }
    public string? LotCode { get; set; }
    public int LotSequence { get; set; }
    public string? Status { get; set; }

    public int TargetQty { get; set; }
    public int ProducedQty { get; set; }
    public int GoodQty { get; set; }
    public int DefectQty { get; set; }

    public double ProgressRate { get; set; }
    public double YieldRate { get; set; }
}