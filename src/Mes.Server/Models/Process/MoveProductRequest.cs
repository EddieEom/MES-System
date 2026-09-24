namespace Mes.Server.Models.Process;

// POST 용 DTO
public class MoveProductRequest
{
    public Guid EventId { get; set; }

    public long ProductId { get; set; }

    public string ToLocationCode { get; set; } = string.Empty;

    public string EventType { get; set; } = "MOVE";

    public DateTimeOffset? EventTime { get; set; }

    public string SourceSystem { get; set; } = "MES";

    public string? Remarks { get; set; }
}