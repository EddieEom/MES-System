namespace Mes.Server.Models.Product;

public class ProductInfo
{
    public long ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public long LotId { get; set; }

    public string LotCode { get; set; } = string.Empty;

    public long WorkOrderId { get; set; }

    public string WorkOrderCode { get; set; } = string.Empty;

    public short SequenceNo { get; set; }

    public string Status { get; set; } = string.Empty;

    public string QualityStatus { get; set; } = string.Empty;

    public int? CurrentLocationId { get; set; }

    public string? CurrentLocationCode { get; set; }

    public string? CurrentLocationName { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }
}