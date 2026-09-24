namespace Mes.Server.Models.Alarm;

public class AlarmInfo
{
    public long AlarmId { get; set; }

    public int MachineId { get; set; }

    public string MachineCode { get; set; } = string.Empty;

    public string AlarmCode { get; set; } = string.Empty;

    public string? AlarmMessage { get; set; }

    public string Severity { get; set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; set; }

    public DateTimeOffset? ClearedAt { get; set; }

    public bool IsActive { get; set; }

    public string SourceSystem { get; set; } = string.Empty;

    public DateTimeOffset ReceivedAt { get; set; }
}