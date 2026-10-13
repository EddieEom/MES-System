using Mes.Hmi.Models.Alarm;
using Mes.Hmi.Models.Control;
using Mes.Hmi.Models.Machine;
using Mes.Hmi.Models.Process;
using Mes.Hmi.Models.Production;
using Mes.Hmi.Models.WorkOrder;
using System.Net;
using System.Net.Http.Json;

namespace Mes.Hmi.Api;

public class MesServerClient
{
    private readonly HttpClient _httpClient;

    public MesServerClient(string baseUrl)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    //=====================================================
    // 현재 생산 정보 조회
    // GET /api/production/current
    //=====================================================
    public async Task<ProductionInfo?> GetCurrentProductionAsync(
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<ProductionInfo>(
            "/api/production/current",
            cancellationToken);
    }

    //=====================================================
    // 현재 공정 제품 조회
    // GET /api/products/process/current
    public async Task<CurrentProductInfo?> GetCurrentProcessProductAsync(
    CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<CurrentProductInfo>(
            "/api/products/process/current",
            cancellationToken);
    }

    //=====================================================
    // 활성 알람 조회
    // GET /api/alarms/active
    //=====================================================
    public async Task<List<AlarmInfo>> GetActiveAlarmsAsync(
    CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<AlarmInfo>>(
            "/api/alarms/active",
            cancellationToken) ?? new List<AlarmInfo>();
    }

    //=====================================================
    // 설비 상태 조회
    // GET /api/machine-status/{machineCode}/latest
    //=====================================================
    public async Task<MachineStatusInfo?> GetLatestMachineStatusAsync(
    string machineCode,
    CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<MachineStatusInfo>(
            $"/api/machine-status/{machineCode}/latest",
            cancellationToken);
    }

    // =====================================================
    // 최근 공정 이벤트 조회
    // GET /api/process/recent?count={count}
    // =====================================================
    public async Task<List<ProcessEventInfo>> GetRecentProcessEventsAsync(
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<ProcessEventInfo>>(
            $"/api/process/recent?count={count}",
            cancellationToken
        ) ?? new List<ProcessEventInfo>();
    }
    
    //=====================================================
    // PLC START 명령
    // POST /api/plc-control/start
    //=====================================================
    public async Task<PlcControlResult?> StartPlcAsync(
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsync(
                "/api/plc-control/start",
                null,
                cancellationToken
            );

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<PlcControlResult>(
                cancellationToken
            );
    }


    //=====================================================
    // PLC STOP 명령
    // POST /api/plc-control/stop
    //=====================================================
    public async Task<PlcControlResult?> StopPlcAsync(
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsync(
                "/api/plc-control/stop",
                null,
                cancellationToken
            );

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<PlcControlResult>(
                cancellationToken
            );
    }


    //=====================================================
    // PLC RESET 명령
    // POST /api/plc-control/reset
    //=====================================================
    public async Task<PlcControlResult?> ResetPlcAsync(
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsync(
                "/api/plc-control/reset",
                null,
                cancellationToken
            );

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<PlcControlResult>(
                cancellationToken
            );
    }

    // =====================================================
    // 미완료 WorkOrder 조회
    // GET /api/work-orders/incomplete
    // =====================================================
    public async Task<WorkOrderIncompleteInfo?>
        GetIncompleteWorkOrderAsync()
    {
        using var response =
            await _httpClient.GetAsync(
                "api/work-orders/incomplete"
            );

        if (response.StatusCode ==
            HttpStatusCode.NoContent)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<WorkOrderIncompleteInfo>();
    }


    // =====================================================
    // 다음 WorkOrder 코드 생성
    //
    // WO-20261013-001
    // WO-20261013-002 ...
    // =====================================================
    public async Task<string>
        GetNextWorkOrderCodeAsync()
    {
        string date =
            DateTime.Now.ToString("yyyyMMdd");

        for (int sequence = 1;
             sequence <= 999;
             sequence++)
        {
            string code =
                $"WO-{date}-{sequence:000}";

            using var response =
                await _httpClient.GetAsync(
                    $"api/work-orders/{code}"
                );

            // 존재하지 않는 코드 발견
            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return code;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"작업지시 코드 확인 실패: {code}"
                );
            }
        }

        throw new InvalidOperationException(
            "생성 가능한 작업지시 코드를 찾지 못했습니다."
        );
    }


    // =====================================================
    // WorkOrder 생성
    // =====================================================
    public async Task CreateWorkOrderAsync(
        string workOrderCode,
        long createdByUserId)
    {
        var request = new
        {
            workOrderCode,
            createdByUserId
        };

        using var response =
            await _httpClient.PostAsJsonAsync(
                "api/work-orders",
                request
            );

        if (!response.IsSuccessStatusCode)
        {
            string body =
                await response.Content
                    .ReadAsStringAsync();

            throw new InvalidOperationException(
                $"작업지시 생성 실패\n{body}"
            );
        }
    }


    // =====================================================
    // WorkOrder START
    // =====================================================
    public async Task StartWorkOrderAsync(
        string workOrderCode)
    {
        using var response =
            await _httpClient.PostAsync(
                $"api/work-orders/{workOrderCode}/start",
                null
            );

        if (!response.IsSuccessStatusCode)
        {
            string body =
                await response.Content
                    .ReadAsStringAsync();

            throw new InvalidOperationException(
                $"작업지시 시작 실패\n{body}"
            );
        }
    }


    // =====================================================
    // WorkOrder RESUME
    // =====================================================
    public async Task ResumeWorkOrderAsync(
        string workOrderCode)
    {
        using var response =
            await _httpClient.PostAsync(
                $"api/work-orders/{workOrderCode}/resume",
                null
            );

        if (!response.IsSuccessStatusCode)
        {
            string body =
                await response.Content
                    .ReadAsStringAsync();

            throw new InvalidOperationException(
                $"작업지시 재개 실패\n{body}"
            );
        }
    }
}