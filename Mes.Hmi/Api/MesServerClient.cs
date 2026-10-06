using System.Net.Http.Json;
using Mes.Hmi.Models.Production;
using Mes.Hmi.Models.Alarm;
using Mes.Hmi.Models.Machine;
using Mes.Hmi.Models.Process;
using Mes.Hmi.Models.Control;

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
}