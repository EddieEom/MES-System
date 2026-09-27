namespace Mes.Gateway.Models.Alarm;

public class AlarmCommandApiResponse
{
    public string Status { get; set; } =
        string.Empty;

    public AlarmApiResponse Alarm { get; set; } =
        new();
}