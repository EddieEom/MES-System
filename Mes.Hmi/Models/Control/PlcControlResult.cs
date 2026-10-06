namespace Mes.Hmi.Models.Control;

public class PlcControlResult
{
    public string? Status { get; set; }

    public Guid CommandId { get; set; }

    public string? Command { get; set; }

    public DateTimeOffset RequestedAt { get; set; }
}