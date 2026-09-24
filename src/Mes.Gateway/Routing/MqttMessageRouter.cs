using Mes.Gateway.Api;
using Mes.Gateway.Models.MachineStatus;
using Mes.Gateway.Models.Process;
using Mes.Gateway.Models.Quailty;
using Mes.Gateway.Services;
using System.Text.Json;

namespace Mes.Gateway.Routing;

public class MqttMessageRouter
{
    private readonly MachineStatusService _machineStatusService;
    private readonly QualityService _qualityService;
    private readonly ProductionService _productionService;
    private readonly MesDashboardService _dashboardService;
    private readonly IMesApiClient _mesApiClient;
    private readonly ActiveProductService _activeProductService;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };


    public MqttMessageRouter(
        MachineStatusService machineStatusService,
        QualityService qualityService,
        ProductionService productionService,
        MesDashboardService dashboardService,
        ActiveProductService activeProductService,
        IMesApiClient mesApiClient)
    {
        _machineStatusService =
            machineStatusService;

        _qualityService =
            qualityService;

        _productionService =
            productionService;

        _dashboardService =
            dashboardService;

        _activeProductService =
        activeProductService;

        _mesApiClient =
            mesApiClient;
    }

    private static bool IsPreQualityLocation(
    string locationCode)
    {
        return locationCode is
            "FEEDER" or
            "CONVEYOR" or
            "ROBOT_TO_VM1" or
            "VM1" or
            "ROBOT_TO_PRESS" or
            "PRESS";
    }

    // ======================================
    // MQTT Topic Routing
    // ======================================

    public async Task RouteAsync(
        string topic,
        string payload)
    {
        try
        {
            switch (topic)
            {
                case "dt/press/status":
                case "dt/vm1/status":

                    await HandleMachineStatusAsync(
                        topic,
                        payload
                    );

                    break;


                case "dt/quality/result":

                    await HandleQualityResultAsync(
                        payload
                    );

                    break;

                case "dt/process/event":

                    await HandleProcessEventAsync(
                        payload
                    );

                    break;

                default:

                    Console.WriteLine(
                        $"[ROUTER] 처리되지 않은 Topic: {topic}"
                    );

                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[ROUTER ERROR] {ex.Message}"
            );
        }
    }

    // process 이벤트 비동기 처리 메서드
    private async Task HandleProcessEventAsync(
    string payload)
    {
        var data =
            JsonSerializer.Deserialize<ProcessEventMessage>(
                payload,
                _jsonOptions
            );


        if (data == null)
        {
            Console.WriteLine(
                "[ROUTER] Process Event 변환 실패"
            );

            return;
        }


        // ======================================
        // EventId 검증
        // ======================================

        if (!Guid.TryParse(
            data.EventId,
            out var eventId))
        {
            Console.WriteLine(
                $"[ROUTER] 잘못된 Process EventId: {data.EventId}"
            );

            return;
        }


        // ======================================
        // 활성 Product 확보
        // ======================================

        var product =
    _activeProductService.CurrentProduct;


        if (product == null)
        {
            // ======================================
            // Gateway 재시작 시
            // DB에 남아있는 IN_PROCESS Product 복구
            // ======================================

            product =
                await _mesApiClient
                    .GetCurrentProcessProductAsync();


            // ======================================
            // 새 제품이 VM1 이전/Press까지 들어오는 상황인데
            // 복구된 Product의 품질판정이 이미 끝났다면
            // 이전 Product가 후단에서 종료되지 않은 stale 상태임
            //
            // 새 공정 이벤트에 재사용하면 안 됨
            // ======================================

            if (product != null
                && !string.Equals(
                    product.QualityStatus,
                    "PENDING",
                    StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine(
                            $"[Gateway] 이미 품질판정 완료된 Product 제외 - {product.ProductCode} / {product.QualityStatus}"
                        );

                product = null;
            }




            // ======================================
            // 기존 진행 Product가 없거나,
            // 이미 품질완료된 stale Product였다면
            // 다음 PENDING Product 확보
            // ======================================

            if (product == null)
            {
                product =
                    await _mesApiClient
                        .GetCurrentQualityProductAsync();
            }


            if (product == null)
            {
                Console.WriteLine(
                    "[Gateway] Quality Result 처리 중단"
                );

                Console.WriteLine(
                    "[Gateway] 품질판정 대상 Product를 찾지 못했습니다."
                );

                return;
            }


            _activeProductService.Set(
                product
            );
        }


        // ======================================
        // Process Event → Mes.Server
        // ======================================

        bool saved =
            await _mesApiClient
                .PostProcessEventAsync(
                    product.ProductId,
                    product.ProductCode,
                    data.ToLocationCode,
                    data.EventType,
                    data.Timestamp,
                    data.Remarks,
                    eventId
                );


        if (!saved)
        {
            Console.WriteLine(
                $"[Gateway] Process Event 저장 실패 - {product.ProductCode} → {data.ToLocationCode}"
            );

            return;
        }


        Console.WriteLine();

        Console.WriteLine(
            "========== PROCESS EVENT =========="
        );

        Console.WriteLine(
            $"EventId     : {data.EventId}"
        );

        Console.WriteLine(
            $"Timestamp   : {data.Timestamp}"
        );

        Console.WriteLine(
            $"ProductId   : {product.ProductId}"
        );

        Console.WriteLine(
            $"ProductCode : {product.ProductCode}"
        );

        Console.WriteLine(
            $"Location    : {data.ToLocationCode}"
        );

        Console.WriteLine(
            $"EventType   : {data.EventType}"
        );

        Console.WriteLine(
            $"Server Saved: {saved}"
        );

        Console.WriteLine(
            "==================================="
        );


        // ======================================
        // 최종 위치 도착 → Product Context 종료
        // ======================================

        if (data.ToLocationCode is
            "WAREHOUSE" or
            "NG_STACK")
        {
            _activeProductService.Clear();
        }
    }

    // ======================================
    // Machine Status
    // ======================================

    private async Task HandleMachineStatusAsync(
        string topic,
        string payload)
    {
        var data =
            JsonSerializer.Deserialize<MachineStatusMessage>(
                payload,
                _jsonOptions
            );


        if (data == null)
        {
            Console.WriteLine(
                "[ROUTER] Machine Status 변환 실패"
            );

            return;
        }


        // ======================================
        // Gateway RAM 최신 상태 갱신
        // ======================================

        _machineStatusService.Update(
            data
        );


        // ======================================
        // Mes.Server → AWS RDS 저장
        // ======================================

        bool saved =
            await _mesApiClient
                .PostMachineStatusAsync(
                    data
                );


        if (!saved)
        {
            Console.WriteLine(
                $"[Gateway] Machine Status 서버 전송 실패 - {data.Machine}"
            );
        }


        // ======================================
        // Console Dashboard
        // ======================================

        _dashboardService.Print();


        // ======================================
        // Machine Status Log
        // ======================================

        Console.WriteLine();

        Console.WriteLine(
            "========== MACHINE STATUS =========="
        );

        Console.WriteLine(
            $"Topic        : {topic}"
        );

        Console.WriteLine(
            $"Timestamp    : {data.Timestamp}"
        );

        Console.WriteLine(
            $"Machine      : {data.Machine}"
        );

        Console.WriteLine(
            $"State        : {data.State}"
        );

        Console.WriteLine(
            $"Entered      : {data.PartsEntered}"
        );

        Console.WriteLine(
            $"Exited       : {data.PartsExited}"
        );

        Console.WriteLine(
            $"Current      : {data.PartsCurrent}"
        );

        Console.WriteLine(
            $"Average Time : {data.PartsAverageTime:F2}"
        );

        Console.WriteLine(
            $"Busy         : {data.BusyPercentage:F2}%"
        );

        Console.WriteLine(
            $"Idle         : {data.IdlePercentage:F2}%"
        );

        Console.WriteLine(
            $"Blocked      : {data.BlockedPercentage:F2}%"
        );

        Console.WriteLine(
            $"Failed       : {data.FailedPercentage:F2}%"
        );

        Console.WriteLine(
            $"Repair       : {data.RepairPercentage:F2}%"
        );

        Console.WriteLine(
            $"Utilization  : {data.Utilization:F2}%"
        );

        Console.WriteLine(
            $"Server Saved : {saved}"
        );

        Console.WriteLine(
            "===================================="
        );
    }


    // ======================================
    // Quality Result
    // ======================================

    private async Task HandleQualityResultAsync(
    string payload)
    {
        var data =
            JsonSerializer.Deserialize<QualityResultMessage>(
                payload,
                _jsonOptions
            );


        if (data == null)
        {
            Console.WriteLine(
                "[ROUTER] Quality Result 변환 실패"
            );

            return;
        }


        // ======================================
        // 1. 현재 활성 Product 확인
        // ======================================

        var product =
            _activeProductService.CurrentProduct;


        // ======================================
        // Process Event 없이 Quality만 들어온 경우
        // 기존 방식으로 Product 확보
        // ======================================

        if (product == null)
        {
            // ======================================
            // Gateway 재시작 후
            // 이미 공정 중인 Product가 있다면 우선 복구
            // ======================================

            product =
                await _mesApiClient
                    .GetCurrentProcessProductAsync();


            // ======================================
            // 공정 중 Product가 없다면
            // 품질판정 대상 Product 조회
            // ======================================

            if (product == null)
            {
                product =
                    await _mesApiClient
                        .GetCurrentQualityProductAsync();
            }


            if (product == null)
            {
                Console.WriteLine(
                    "[Gateway] Quality Result 처리 중단"
                );

                Console.WriteLine(
                    "[Gateway] 품질판정 대상 Product를 찾지 못했습니다."
                );

                return;
            }


            _activeProductService.Set(
                product
            );
        }

        // ======================================
        // 2. Quality Result → Mes.Server 저장
        // ======================================

        bool saved =
            await _mesApiClient
                .PostQualityResultAsync(
                    data,
                    product.ProductId
                );


        if (!saved)
        {
            Console.WriteLine(
                $"[Gateway] Quality Result 서버 전송 실패 - EventId={data.EventId}"
            );

            return;
        }

        // ======================================
        // 품질 결과에 따른 최종 위치 결정
        // ======================================

        string finalLocation;

        if (string.Equals(
            data.Result,
            "PASS",
            StringComparison.OrdinalIgnoreCase))
        {
            finalLocation = "WAREHOUSE";
        }
        else if (string.Equals(
            data.Result,
            "FAIL",
            StringComparison.OrdinalIgnoreCase))
        {
            finalLocation = "NG_STACK";
        }
        else
        {
            Console.WriteLine(
                $"[Gateway] 알 수 없는 Quality Result: {data.Result}"
            );

            return;
        }


        // ======================================
        // MES 논리상 최종 위치 이동
        // ======================================

        bool finalMoveSaved =
            await _mesApiClient
                .PostProcessEventAsync(
                    product.ProductId,
                    product.ProductCode,
                    finalLocation,
                    "ROUTE",
                    data.Timestamp,
                    $"Quality {data.Result} 결과에 따른 최종 경로"
                );


        if (!finalMoveSaved)
        {
            Console.WriteLine(
                $"[Gateway] 최종 위치 저장 실패 - {product.ProductCode} → {finalLocation}"
            );

            return;
        }


        // ======================================
        // Product 종료 → Context 해제
        // ======================================

        _activeProductService.Clear();

        // ======================================
        // 3. Gateway 임시 RAM 집계
        //
        // 서버 저장이 성공했을 때만 갱신
        // 최종적으로는 DB 기반 집계로 교체 예정
        // ======================================

        bool processed =
            _qualityService.Process(
                data
            );


        if (processed)
        {
            _productionService.PrintStatus();

            _dashboardService.Print();
        }


        // ======================================
        // Quality Result Log
        // ======================================

        Console.WriteLine();

        Console.WriteLine(
            "========== QUALITY RESULT =========="
        );

        Console.WriteLine(
            $"EventId     : {data.EventId}"
        );

        Console.WriteLine(
            $"Timestamp   : {data.Timestamp}"
        );

        Console.WriteLine(
            $"Machine     : {data.Machine}"
        );

        Console.WriteLine(
            $"Result      : {data.Result}"
        );

        Console.WriteLine(
            $"Type        : {data.ProductType}"
        );

        Console.WriteLine(
            $"ProductId   : {product.ProductId}"
        );

        Console.WriteLine(
            $"ProductCode : {product.ProductCode}"
        );

        Console.WriteLine(
            $"LotCode     : {product.LotCode}"
        );

        Console.WriteLine(
            $"WorkOrder   : {product.WorkOrderCode}"
        );

        Console.WriteLine(
            $"Server Saved: {saved}"
        );

        Console.WriteLine(
            "===================================="
        );
    }
}