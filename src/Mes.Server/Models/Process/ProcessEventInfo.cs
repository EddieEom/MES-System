namespace Mes.Server.Models.Process;

// 제품 추적 조회용
public class ProcessEventInfo
{
    public long ProcessEventId { get; set; }

    public Guid EventId { get; set; }

    public long ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public int? MachineId { get; set; }

    public string? MachineCode { get; set; }

    public string EventType { get; set; } = string.Empty;

    public int? FromLocationId { get; set; }

    public string? FromLocationCode { get; set; }

    public int? ToLocationId { get; set; }

    public string? ToLocationCode { get; set; }

    public DateTimeOffset EventTime { get; set; }

    public string SourceSystem { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}