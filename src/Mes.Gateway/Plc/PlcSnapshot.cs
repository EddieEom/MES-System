namespace Mes.Gateway.Plc;

public class PlcSnapshot
{
    public int StateCode { get; set; }

    public int AlarmCode { get; set; }

    public DateTimeOffset ReadAt { get; set; }


    public string State =>
        StateCode switch
        {
            0 => "STOP",
            1 => "RUN",
            2 => "ALARM",
            _ => "UNKNOWN"
        };
}