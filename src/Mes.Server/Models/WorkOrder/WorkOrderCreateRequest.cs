namespace Mes.Server.Models.WorkOrder;

// POST 입력 전용 DTO.
public class WorkOrderCreateRequest
{
    public string WorkOrderCode { get; set; } = string.Empty;

    public long? CreatedByUserId { get; set; }
}