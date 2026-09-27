namespace Mes.Gateway.Models.Control;

public class PlcCommandMessage
{
    public Guid CommandId { get; set; }

    public string Command { get; set; } =
        string.Empty;

    public DateTimeOffset RequestedAt { get; set; }
}