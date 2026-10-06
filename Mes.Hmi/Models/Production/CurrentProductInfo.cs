namespace Mes.Hmi.Models.Production;

public class CurrentProductInfo
{
    public long ProductId { get; set; }
    public string? ProductCode { get; set; }

    public long LotId { get; set; }
    public string? LotCode { get; set; }

    public long WorkOrderId { get; set; }
    public string? WorkOrderCode { get; set; }

    public int SequenceNo { get; set; }

    public string? Status { get; set; }
    public string? QualityStatus { get; set; }

    public int CurrentLocationId { get; set; }
    public string? CurrentLocationCode { get; set; }
    public string? CurrentLocationName { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}