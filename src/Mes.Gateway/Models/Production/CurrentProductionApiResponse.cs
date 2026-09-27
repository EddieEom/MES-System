namespace Mes.Gateway.Models.Production;

public class CurrentProductionApiResponse
{
    public WorkOrderProductionApiResponse WorkOrder { get; set; } =
        new();

    public LotProductionApiResponse? CurrentLot { get; set; }
}


public class WorkOrderProductionApiResponse
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

    public double AchievementRate { get; set; }

    public double YieldRate { get; set; }
}


public class LotProductionApiResponse
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

    public double ProgressRate { get; set; }

    public double YieldRate { get; set; }
}