namespace Mes.Server.Models.WorkOrder;

public class WorkOrderCreateResult
{
    public long WorkOrderId { get; set; }

    public string WorkOrderCode { get; set; } = string.Empty;

    public int LotCount { get; set; }

    public int ProductCount { get; set; }
}