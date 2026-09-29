namespace Mes.Hmi.Models.Production;

public class ProductionInfo
{
    public WorkOrderProductionInfo WorkOrder { get; set; } = new();

    public LotProductionInfo? CurrentLot { get; set; }
}


public class WorkOrderProductionInfo
{
    public string WorkOrderCode { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int TargetQty { get; set; }

    public int ProducedQty { get; set; }

    public int GoodQty { get; set; }

    public int DefectQty { get; set; }

    public decimal AchievementRate { get; set; }

    public decimal YieldRate { get; set; }
}


public class LotProductionInfo
{
    public string LotCode { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int TargetQty { get; set; }

    public int ProducedQty { get; set; }

    public int GoodQty { get; set; }

    public int DefectQty { get; set; }

    public decimal ProgressRate { get; set; }

    public decimal YieldRate { get; set; }
}