namespace Mes.Server.Models.WorkOrder;

public class WorkOrderStartResult
{
    public long WorkOrderId { get; set; }

    public string WorkOrderCode { get; set; } = string.Empty;

    public string WorkOrderStatus { get; set; } = string.Empty;

    public DateTimeOffset WorkOrderStartedAt { get; set; }


    public long LotId { get; set; }

    public string LotCode { get; set; } = string.Empty;

    public string LotStatus { get; set; } = string.Empty;

    public DateTimeOffset LotStartedAt { get; set; }
}