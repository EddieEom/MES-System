namespace Mes.Hmi.Models.Alarm;

public class AlarmInfo
{
    public int AlarmId { get; set; }
    public int MachineId { get; set; }
    public string? MachineCode { get; set; }

    public string? AlarmCode { get; set; }
    public string? AlarmMessage { get; set; }
    public string? Severity { get; set; }

    public DateTimeOffset OccurredAt { get; set; }
    public DateTimeOffset? ClearedAt { get; set; }

    public bool IsActive { get; set; }

    public string? SourceSystem { get; set; }

    public DateTimeOffset ReceivedAt { get; set; }
}