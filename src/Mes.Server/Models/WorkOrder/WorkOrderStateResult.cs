namespace Mes.Server.Models.WorkOrder;

public class WorkOrderStateResult
{
    public long WorkOrderId { get; set; }

    public string WorkOrderCode { get; set; } = string.Empty;

    public string WorkOrderStatus { get; set; } = string.Empty;

    public DateTimeOffset? WorkOrderStartedAt { get; set; }


    public long CurrentLotId { get; set; }

    public string CurrentLotCode { get; set; } = string.Empty;

    public string CurrentLotStatus { get; set; } = string.Empty;
}