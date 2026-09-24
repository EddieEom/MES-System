namespace Mes.Gateway.Models.MoveProduct;

public class MoveProductApiRequest
{
    public Guid EventId { get; set; }

    public long ProductId { get; set; }

    public string ToLocationCode { get; set; } = string.Empty;

    public string EventType { get; set; } = "MOVE";

    public DateTimeOffset? EventTime { get; set; }

    public string SourceSystem { get; set; } = "GEMINI";

    public string? Remarks { get; set; }
}