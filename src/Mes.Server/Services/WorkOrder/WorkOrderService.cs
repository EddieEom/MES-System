using Mes.Server.Models.WorkOrder;
using Mes.Server.Repositories.WorkOrder;

namespace Mes.Server.Services.WorkOrder;

// 나중에 Gateway 명령, SignalR 명령, 생산 컨텍스트 묶기
public class WorkOrderService
    : IWorkOrderService
{
    private readonly IWorkOrderRepository _repository;

    public WorkOrderService(
        IWorkOrderRepository repository)
    {
        _repository = repository;
    }


    public async Task<WorkOrderStartResult> StartAsync(
        string workOrderCode)
    {
        if (string.IsNullOrWhiteSpace(
            workOrderCode))
        {
            throw new ArgumentException(
                "WorkOrderCode는 필수입니다."
            );
        }


        return await _repository.StartAsync(
            workOrderCode
        );
    }

    public async Task<WorkOrderStateResult> PauseAsync(
    string workOrderCode)
    {
        if (string.IsNullOrWhiteSpace(
            workOrderCode))
        {
            throw new ArgumentException(
                "WorkOrderCode는 필수입니다."
            );
        }


        return await _repository.PauseAsync(
            workOrderCode
        );
    }


    public async Task<WorkOrderStateResult> ResumeAsync(
        string workOrderCode)
    {
        if (string.IsNullOrWhiteSpace(
            workOrderCode))
        {
            throw new ArgumentException(
                "WorkOrderCode는 필수입니다."
            );
        }


        return await _repository.ResumeAsync(
            workOrderCode
        );
    }
}