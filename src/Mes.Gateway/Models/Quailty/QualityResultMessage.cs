namespace Mes.Gateway.Models.Quailty;

public class QualityResultMessage
{
    public string EventId { get; set; } = string.Empty;

    public DateTimeOffset Timestamp { get; set; }

    public string Machine { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public string ProductType { get; set; } = string.Empty;

    public int ProductTypeId { get; set; }
}