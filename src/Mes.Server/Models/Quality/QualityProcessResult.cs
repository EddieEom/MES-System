namespace Mes.Server.Models.Quality;

public class QualityProcessResult
{
    public string WorkOrderCode { get; set; } = string.Empty;

    public string WorkOrderStatus { get; set; } = string.Empty;

    public string LotCode { get; set; } = string.Empty;

    public string LotStatus { get; set; } = string.Empty;

    public int ProcessedQty { get; set; }

    public int TargetQty { get; set; }

    public int GoodQty { get; set; }

    public int DefectQty { get; set; }
}