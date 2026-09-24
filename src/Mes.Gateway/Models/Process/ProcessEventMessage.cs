namespace Mes.Gateway.Models.Process;

public class ProcessEventMessage
{
    public string EventId { get; set; } = string.Empty;

    public DateTimeOffset Timestamp { get; set; }

    public string ToLocationCode { get; set; } =
        string.Empty;

    public string EventType { get; set; } =
        "MOVE";

    public string? Remarks { get; set; }
}