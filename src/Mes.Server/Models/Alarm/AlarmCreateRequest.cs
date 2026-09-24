namespace Mes.Server.Models.Alarm;

// POST 용 DTO
public class AlarmCreateRequest
{
    public string MachineCode { get; set; } = string.Empty;

    public string AlarmCode { get; set; } = string.Empty;

    public string? AlarmMessage { get; set; }

    public string Severity { get; set; } = "WARNING";

    public DateTimeOffset OccurredAt { get; set; }

    public string SourceSystem { get; set; } = "PLC";
}