using Mes.Gateway.Models.MachineStatus;
using Mes.Gateway.Models.MoveProduct;
using Mes.Gateway.Models.Quailty;
using Mes.Gateway.Services;
using System.Net;
using System.Net.Http.Json;
using Mes.Gateway.Models.Production;
using Mes.Gateway.Models.Alarm;

namespace Mes.Gateway.Api;

public class MesApiClient : IMesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly FailedEventStore _failedEventStore;

    private static readonly TimeSpan RequestTimeout =
        TimeSpan.FromSeconds(5);


    public MesApiClient(
        HttpClient httpClient,
        FailedEventStore failedEventStore)
    {
        _httpClient = httpClient;
        _failedEventStore = failedEventStore;
    }


    // =====================================================
    // Alarm 발생
    //
    // Alarm에는 EventId가 없으므로
    // 자동 Retry를 사용하지 않음.
    // =====================================================

    public async Task<AlarmApiResponse?> PostAlarmAsync(
        AlarmCreateApiRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response =
                await SendWithRetryAsync(
                    ct =>
                        _httpClient.PostAsJsonAsync(
                            "api/alarms",
                            request,
                            ct
                        ),

                    $"Alarm Create/{request.MachineCode}/{request.AlarmCode}",

                    enableRetry: false,

                    cancellationToken
                );


            // ==========================================
            // 응답 유실 가능성
            //
            // Retry하면 중복 INSERT 위험이 있으므로
            // ACTIVE Alarm을 조회해 방금 저장된 Alarm 복구
            // ==========================================

            if (response == null)
            {
                var recovered =
                    await FindActiveAlarmAsync(
                        request.MachineCode,
                        request.AlarmCode,
                        request.OccurredAt,
                        cancellationToken
                    );


                if (recovered != null)
                {
                    Console.WriteLine(
                        $"[MES API] Alarm DB 저장 확인 완료 - {recovered.MachineCode}/{recovered.AlarmCode} / ID={recovered.AlarmId}"
                    );

                    return recovered;
                }


                await _failedEventStore
                    .SaveAsync(
                        "alarm-create",
                        request,
                        "HTTP 연결 실패 또는 Timeout"
                    );


                return null;
            }


            var body =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken
                    );


            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    "[MES API] Alarm 등록 실패"
                );

                Console.WriteLine(
                    $"[MES API] Machine: {request.MachineCode}"
                );

                Console.WriteLine(
                    $"[MES API] AlarmCode: {request.AlarmCode}"
                );

                Console.WriteLine(
                    $"[MES API] HTTP {(int)response.StatusCode} {response.StatusCode}"
                );

                Console.WriteLine(
                    $"[MES API] Response: {body}"
                );


                await _failedEventStore
                    .SaveAsync(
                        "alarm-create",
                        request,
                        body
                    );


                return null;
            }


            var result =
                System.Text.Json.JsonSerializer
                    .Deserialize<AlarmCommandApiResponse>(
                        body,
                        new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    );


            if (result?.Alarm == null)
            {
                await _failedEventStore
                    .SaveAsync(
                        "alarm-create",
                        request,
                        "Alarm 응답 변환 실패"
                    );


                return null;
            }


            Console.WriteLine(
                $"[MES API] Alarm 등록 성공 - {result.Alarm.MachineCode}/{result.Alarm.AlarmCode} / ID={result.Alarm.AlarmId}"
            );


            return result.Alarm;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[MES API] Alarm 등록 오류 - {request.MachineCode}/{request.AlarmCode}: {ex.Message}"
            );


            await _failedEventStore
                .SaveAsync(
                    "alarm-create",
                    request,
                    ex.Message
                );


            return null;
        }
    }


    // =====================================================
    // 현재 ACTIVE Alarm 조회
    // =====================================================

    public async Task<IReadOnlyList<AlarmApiResponse>?>
        GetActiveAlarmsAsync(
            CancellationToken cancellationToken = default)
    {
        using var response =
            await SendWithRetryAsync(
                ct =>
                    _httpClient.GetAsync(
                        "api/alarms/active",
                        ct
                    ),

                "ACTIVE Alarm 조회",

                enableRetry: true,

                cancellationToken
            );


        if (response == null)
        {
            Console.WriteLine(
                "[MES API] ACTIVE Alarm 조회 실패 - 응답 없음"
            );

            return null;
        }


        if (!response.IsSuccessStatusCode)
        {
            var body =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken
                    );


            Console.WriteLine(
                $"[MES API] ACTIVE Alarm 조회 실패 - HTTP {(int)response.StatusCode}"
            );

            Console.WriteLine(
                $"[MES API] Response: {body}"
            );


            return null;
        }


        var alarms =
            await response.Content
                .ReadFromJsonAsync<
                    List<AlarmApiResponse>
                >(
                    cancellationToken:
                        cancellationToken
                );


        return alarms
            ?? new List<AlarmApiResponse>();
    }


    // =====================================================
    // 설비별 Alarm History
    // =====================================================

    public async Task<IReadOnlyList<AlarmApiResponse>?>
        GetMachineAlarmsAsync(
            string machineCode,
            CancellationToken cancellationToken = default)
    {
        string encodedMachineCode =
            Uri.EscapeDataString(
                machineCode
            );


        using var response =
            await SendWithRetryAsync(
                ct =>
                    _httpClient.GetAsync(
                        $"api/alarms/machine/{encodedMachineCode}",
                        ct
                    ),

                $"Alarm History/{machineCode}",

                enableRetry: true,

                cancellationToken
            );


        if (response == null)
        {
            return null;
        }


        if (!response.IsSuccessStatusCode)
        {
            var body =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken
                    );


            Console.WriteLine(
                $"[MES API] Alarm History 조회 실패 - {machineCode}"
            );

            Console.WriteLine(
                $"[MES API] Response: {body}"
            );


            return null;
        }


        var alarms =
            await response.Content
                .ReadFromJsonAsync<
                    List<AlarmApiResponse>
                >(
                    cancellationToken:
                        cancellationToken
                );


        return alarms
            ?? new List<AlarmApiResponse>();
    }


    // =====================================================
    // Alarm Clear
    // =====================================================

    public async Task<bool> ClearAlarmAsync(
        long alarmId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response =
                await SendWithRetryAsync(
                    ct =>
                        _httpClient.PostAsync(
                            $"api/alarms/{alarmId}/clear",
                            null,
                            ct
                        ),

                    $"Alarm Clear/{alarmId}",

                    // 응답 유실 후 재호출 시
                    // Server가 이미 Clear된 Alarm에 404를 반환하므로
                    // 자동 Retry하지 않음.
                    enableRetry: false,

                    cancellationToken
                );


            if (response == null)
            {
                // Clear 요청은 Server에서 성공했는데
                // HTTP 응답만 유실됐을 수 있음.
                var active =
                    await GetActiveAlarmsAsync(
                        cancellationToken
                    );


                if (active != null &&
                    !active.Any(
                        x =>
                            x.AlarmId == alarmId
                            && x.IsActive
                    ))
                {
                    Console.WriteLine(
                        $"[MES API] Alarm Clear DB 반영 확인 - ID={alarmId}"
                    );

                    return true;
                }


                await _failedEventStore
                    .SaveAsync(
                        "alarm-clear",
                        new
                        {
                            AlarmId = alarmId
                        },
                        "HTTP 연결 실패 또는 Timeout"
                    );


                return false;
            }


            var body =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken
                    );


            if (!response.IsSuccessStatusCode)
            {
                // 다른 주체가 먼저 Clear했을 수도 있으므로
                // ACTIVE 목록에서 최종 상태 확인
                var active =
                    await GetActiveAlarmsAsync(
                        cancellationToken
                    );


                if (active != null &&
                    !active.Any(
                        x =>
                            x.AlarmId == alarmId
                            && x.IsActive
                    ))
                {
                    Console.WriteLine(
                        $"[MES API] Alarm 이미 Clear 상태 - ID={alarmId}"
                    );

                    return true;
                }


                Console.WriteLine(
                    $"[MES API] Alarm Clear 실패 - ID={alarmId}"
                );

                Console.WriteLine(
                    $"[MES API] Response: {body}"
                );


                await _failedEventStore
                    .SaveAsync(
                        "alarm-clear",
                        new
                        {
                            AlarmId = alarmId
                        },
                        body
                    );


                return false;
            }


            Console.WriteLine(
                $"[MES API] Alarm Clear 성공 - ID={alarmId}"
            );


            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[MES API] Alarm Clear 오류 - ID={alarmId}: {ex.Message}"
            );


            await _failedEventStore
                .SaveAsync(
                    "alarm-clear",
                    new
                    {
                        AlarmId = alarmId
                    },
                    ex.Message
                );


            return false;
        }
    }


    // =====================================================
    // 응답 유실 후 ACTIVE Alarm 복구
    // =====================================================

    private async Task<AlarmApiResponse?>
        FindActiveAlarmAsync(
            string machineCode,
            string alarmCode,
            DateTimeOffset occurredAt,
            CancellationToken cancellationToken)
    {
        var alarms =
            await GetActiveAlarmsAsync(
                cancellationToken
            );


        if (alarms == null)
        {
            return null;
        }


        return alarms.FirstOrDefault(
            x =>
                x.IsActive

                && string.Equals(
                    x.MachineCode,
                    machineCode,
                    StringComparison.OrdinalIgnoreCase
                )

                && string.Equals(
                    x.AlarmCode,
                    alarmCode,
                    StringComparison.OrdinalIgnoreCase
                )

                // 기존의 오래된 동일 Alarm을
                // 방금 요청한 Alarm으로 오인하지 않도록 제한
                && Math.Abs(
                    (
                        x.OccurredAt
                        - occurredAt
                    ).TotalSeconds
                ) <= 10
        );
    }

    // =====================================================
    // 현재 DB 기준 생산실적 조회
    // =====================================================

    public async Task<CurrentProductionApiResponse?>
        GetCurrentProductionAsync(
            CancellationToken cancellationToken = default)
    {
        using var response =
            await SendWithRetryAsync(
                ct =>
                    _httpClient.GetAsync(
                        "api/production/current",
                        ct
                    ),

                "현재 생산실적 조회",

                enableRetry: true,

                cancellationToken
            );


        if (response == null)
        {
            Console.WriteLine(
                "[MES API] 현재 생산실적 조회 실패 - 응답 없음"
            );

            return null;
        }


        if (response.StatusCode ==
            HttpStatusCode.NotFound)
        {
            Console.WriteLine(
                "[MES API] 현재 활성 생산정보 없음"
            );

            return null;
        }


        if (!response.IsSuccessStatusCode)
        {
            var body =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken
                    );


            Console.WriteLine(
                "[MES API] 현재 생산실적 조회 실패"
            );

            Console.WriteLine(
                $"[MES API] HTTP {(int)response.StatusCode} {response.StatusCode}"
            );

            Console.WriteLine(
                $"[MES API] Response: {body}"
            );


            return null;
        }


        var production =
            await response.Content
                .ReadFromJsonAsync<
                    CurrentProductionApiResponse
                >(
                    cancellationToken:
                        cancellationToken
                );


        if (production == null)
        {
            Console.WriteLine(
                "[MES API] 생산실적 응답 변환 실패"
            );

            return null;
        }


        Console.WriteLine(
            $"[MES API] 생산실적 조회 성공 - {production.WorkOrder.WorkOrderCode}"
        );

        Console.WriteLine(
            $"[MES API] Produced={production.WorkOrder.ProducedQty} / Good={production.WorkOrder.GoodQty} / Defect={production.WorkOrder.DefectQty}"
        );


        return production;
    }

    // =====================================================
    // 공통 Product GET
    // =====================================================

    private async Task<ProductApiResponse?> GetProductAsync(
        string path,
        string description,
        CancellationToken cancellationToken)
    {
        using var response =
            await SendWithRetryAsync(
                ct => _httpClient.GetAsync(
                    path,
                    ct
                ),
                description,
                enableRetry: true,
                cancellationToken
            );


        if (response == null)
        {
            Console.WriteLine(
                $"[MES API] {description} 실패 - 응답 없음"
            );

            return null;
        }


        if (response.StatusCode ==
            HttpStatusCode.NotFound)
        {
            return null;
        }


        if (!response.IsSuccessStatusCode)
        {
            var body =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken
                    );


            Console.WriteLine(
                $"[MES API] {description} 실패"
            );

            Console.WriteLine(
                $"[MES API] HTTP {(int)response.StatusCode} {response.StatusCode}"
            );

            Console.WriteLine(
                $"[MES API] Response: {body}"
            );


            return null;
        }


        var product =
            await response.Content
                .ReadFromJsonAsync<ProductApiResponse>(
                    cancellationToken:
                        cancellationToken
                );


        return product;
    }


    // =====================================================
    // 일시적 HTTP 오류 여부
    // =====================================================

    private static bool IsTransientStatus(
        HttpStatusCode statusCode)
    {
        int code = (int)statusCode;

        return
            statusCode == HttpStatusCode.RequestTimeout
            || code == 429
            || code >= 500;
    }


    // =====================================================
    // 공통 Retry + Timeout
    //
    // Retry:
    // 1차
    // → 1초
    // → 2차
    // → 2초
    // → 3차
    //
    // 각 요청 Timeout = 5초
    // =====================================================

    private async Task<HttpResponseMessage?>
        SendWithRetryAsync(
            Func<CancellationToken, Task<HttpResponseMessage>>
                sendAsync,
            string operationName,
            bool enableRetry,
            CancellationToken cancellationToken)
    {
        int maxAttempts =
            enableRetry
                ? 3
                : 1;


        for (
            int attempt = 1;
            attempt <= maxAttempts;
            attempt++)
        {
            using var timeoutCts =
                CancellationTokenSource
                    .CreateLinkedTokenSource(
                        cancellationToken
                    );


            timeoutCts.CancelAfter(
                RequestTimeout
            );


            try
            {
                var response =
                    await sendAsync(
                        timeoutCts.Token
                    );


                // 재시도 비활성 API
                if (!enableRetry)
                {
                    return response;
                }


                // 400, 404 등은 일시적 장애가 아니므로
                // 재시도하지 않음
                if (!IsTransientStatus(
                    response.StatusCode))
                {
                    return response;
                }


                // 마지막 시도
                if (attempt == maxAttempts)
                {
                    return response;
                }


                Console.WriteLine(
                    $"[MES API] {operationName} 일시적 실패 - HTTP {(int)response.StatusCode}"
                );

                Console.WriteLine(
                    $"[MES API] 재시도 예정 ({attempt}/{maxAttempts})"
                );


                response.Dispose();
            }
            catch (OperationCanceledException)
                when (!cancellationToken
                    .IsCancellationRequested)
            {
                Console.WriteLine(
                    $"[MES API] {operationName} Timeout - {attempt}/{maxAttempts}"
                );


                if (attempt == maxAttempts)
                {
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(
                    $"[MES API] {operationName} 연결 오류 - {ex.Message}"
                );


                if (attempt == maxAttempts)
                {
                    return null;
                }
            }


            var delay =
                TimeSpan.FromSeconds(
                    attempt
                );


            Console.WriteLine(
                $"[MES API] {delay.TotalSeconds:0}초 후 재시도"
            );


            await Task.Delay(
                delay,
                cancellationToken
            );
        }


        return null;
    }


    // =====================================================
    // Server Health
    // =====================================================

    public async Task<bool> CheckConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response =
                await SendWithRetryAsync(
                    ct => _httpClient.GetAsync(
                        "api/database/health",
                        ct
                    ),
                    "Mes.Server 연결 확인",
                    enableRetry: true,
                    cancellationToken
                );


            if (response == null)
            {
                Console.WriteLine(
                    "[MES API] Mes.Server 연결 실패 - 응답 없음"
                );

                return false;
            }


            if (response.StatusCode !=
                HttpStatusCode.OK)
            {
                var body =
                    await response.Content
                        .ReadAsStringAsync(
                            cancellationToken
                        );


                Console.WriteLine(
                    $"[MES API] 연결 실패 - HTTP {(int)response.StatusCode} {response.StatusCode}"
                );

                Console.WriteLine(
                    $"[MES API] Response: {body}"
                );


                return false;
            }


            var responseBody =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken
                    );


            Console.WriteLine(
                "[MES API] Mes.Server 연결 성공"
            );

            Console.WriteLine(
                $"[MES API] Response: {responseBody}"
            );


            return true;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine(
                "[MES API] Mes.Server 연결 확인 취소"
            );

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[MES API] Mes.Server 연결 확인 오류: {ex.Message}"
            );

            return false;
        }
    }


    // =====================================================
    // Machine Status POST
    //
    // Machine Status에는 고유 EventId가 없으므로
    // 자동 Retry를 하지 않음.
    // =====================================================

    public async Task<bool> PostMachineStatusAsync(
        MachineStatusMessage message,
        CancellationToken cancellationToken = default)
    {
        var request =
            new MachineStatusApiRequest
            {
                MachineCode =
                    message.Machine switch
                    {
                        "VM-1" => "VM1",
                        "PRESS" => "PRESS",
                        _ => message.Machine
                    },

                MeasuredAt =
                    message.Timestamp,

                State =
                    message.State,

                PartsEntered =
                    message.PartsEntered,

                PartsExited =
                    message.PartsExited,

                PartsCurrent =
                    message.PartsCurrent,

                PartsAverageTime =
                    message.PartsAverageTime,

                IdlePercentage =
                    message.IdlePercentage,

                BusyPercentage =
                    message.BusyPercentage,

                BlockedPercentage =
                    message.BlockedPercentage,

                FailedPercentage =
                    message.FailedPercentage,

                RepairPercentage =
                    message.RepairPercentage,

                Utilization =
                    message.Utilization,

                SourceSystem =
                    "GEMINI"
            };


        try
        {
            using var response =
                await SendWithRetryAsync(
                    ct =>
                        _httpClient.PostAsJsonAsync(
                            "api/machine-status",
                            request,
                            ct
                        ),
                    $"MachineStatus/{message.Machine}",
                    enableRetry: false,
                    cancellationToken
                );


            if (response == null)
            {
                Console.WriteLine(
                    $"[MES API] Machine Status 전송 실패 - {message.Machine}"
                );


                await _failedEventStore
                    .SaveAsync(
                        "machine-status",
                        request,
                        "HTTP 연결 실패 또는 Timeout"
                    );


                return false;
            }


            if (!response.IsSuccessStatusCode)
            {
                var errorBody =
                    await response.Content
                        .ReadAsStringAsync(
                            cancellationToken
                        );


                Console.WriteLine(
                    "[MES API] Machine Status 저장 실패"
                );

                Console.WriteLine(
                    $"[MES API] Machine: {message.Machine}"
                );

                Console.WriteLine(
                    $"[MES API] HTTP {(int)response.StatusCode} {response.StatusCode}"
                );

                Console.WriteLine(
                    $"[MES API] Response: {errorBody}"
                );


                await _failedEventStore
                    .SaveAsync(
                        "machine-status",
                        request,
                        errorBody
                    );


                return false;
            }


            Console.WriteLine(
                $"[MES API] Machine Status 저장 성공 - {message.Machine} / {message.State}"
            );


            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[MES API] Machine Status 오류 - {message.Machine}: {ex.Message}"
            );


            await _failedEventStore
                .SaveAsync(
                    "machine-status",
                    request,
                    ex.Message
                );


            return false;
        }
    }


    // =====================================================
    // 현재 품질판정 대상 Product
    // =====================================================

    public async Task<ProductApiResponse?>
        GetCurrentQualityProductAsync(
            CancellationToken cancellationToken = default)
    {
        var product =
            await GetProductAsync(
                "api/products/quality/current",
                "품질 대상 Product 조회",
                cancellationToken
            );


        if (product != null)
        {
            Console.WriteLine(
                $"[MES API] 품질 대상 Product 확인 - {product.ProductCode} / ID={product.ProductId}"
            );
        }


        return product;
    }


    // =====================================================
    // 현재 IN_PROCESS Product
    //
    // Gateway 재시작 시 DB에서 Product Context 복구
    // =====================================================

    public async Task<ProductApiResponse?>
        GetCurrentProcessProductAsync(
            CancellationToken cancellationToken = default)
    {
        var product =
            await GetProductAsync(
                "api/products/process/current",
                "현재 IN_PROCESS Product 조회",
                cancellationToken
            );


        if (product != null)
        {
            Console.WriteLine(
                $"[MES API] 진행 중 Product 복구 - {product.ProductCode} / ID={product.ProductId} / Location={product.CurrentLocationCode}"
            );
        }


        return product;
    }


    // =====================================================
    // Quality Event 존재 확인
    //
    // POST 응답 유실 후 실제 DB 저장 여부 검증
    // =====================================================

    private async Task<bool> QualityEventExistsAsync(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        using var response =
            await SendWithRetryAsync(
                ct =>
                    _httpClient.GetAsync(
                        $"api/quality-results/{eventId}",
                        ct
                    ),
                $"Quality Event 확인/{eventId}",
                enableRetry: true,
                cancellationToken
            );


        if (response == null)
        {
            return false;
        }


        if (response.StatusCode ==
            HttpStatusCode.NotFound)
        {
            return false;
        }


        return response.IsSuccessStatusCode;
    }


    // =====================================================
    // Quality Result POST
    //
    // EventId 기반 Idempotent Retry
    // =====================================================

    public async Task<bool> PostQualityResultAsync(
        QualityResultMessage message,
        long productId,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(
            message.EventId,
            out var eventId))
        {
            Console.WriteLine(
                $"[MES API] 잘못된 Quality EventId 형식: {message.EventId}"
            );


            await _failedEventStore
                .SaveAsync(
                    "quality",
                    message,
                    $"잘못된 EventId 형식: {message.EventId}"
                );


            return false;
        }


        var request =
            new QualityResultApiRequest
            {
                EventId =
                    eventId,

                ProductId =
                    productId,

                Result =
                    message.Result,

                SourceProductType =
                    message.ProductType,

                InspectedAt =
                    message.Timestamp,

                SourceSystem =
                    "GEMINI"
            };


        try
        {
            using var response =
                await SendWithRetryAsync(
                    ct =>
                        _httpClient.PostAsJsonAsync(
                            "api/quality-results",
                            request,
                            ct
                        ),
                    $"Quality/{eventId}",
                    enableRetry: true,
                    cancellationToken
                );


            // -----------------------------------------
            // 응답 자체를 받지 못한 경우
            //
            // 요청이 Server까지 도착해 Commit됐지만
            // 응답만 유실됐을 가능성이 있으므로 확인
            // -----------------------------------------

            if (response == null)
            {
                if (await QualityEventExistsAsync(
                    eventId,
                    cancellationToken))
                {
                    Console.WriteLine(
                        $"[MES API] Quality Event DB 저장 확인 완료 - {eventId}"
                    );


                    return true;
                }


                await _failedEventStore
                    .SaveAsync(
                        "quality",
                        request,
                        "최종 HTTP 연결 실패 또는 Timeout"
                    );


                return false;
            }


            var body =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken
                    );


            // -----------------------------------------
            // HTTP 실패
            //
            // Duplicate EventId 등이면
            // DB 실제 존재 여부를 다시 확인
            // -----------------------------------------

            if (!response.IsSuccessStatusCode)
            {
                if (await QualityEventExistsAsync(
                    eventId,
                    cancellationToken))
                {
                    Console.WriteLine(
                        $"[MES API] Quality Event 이미 DB에 존재 - {eventId}"
                    );


                    return true;
                }


                Console.WriteLine(
                    "[MES API] Quality Result 저장 실패"
                );

                Console.WriteLine(
                    $"[MES API] EventId: {eventId}"
                );

                Console.WriteLine(
                    $"[MES API] ProductId: {productId}"
                );

                Console.WriteLine(
                    $"[MES API] HTTP {(int)response.StatusCode} {response.StatusCode}"
                );

                Console.WriteLine(
                    $"[MES API] Response: {body}"
                );


                await _failedEventStore
                    .SaveAsync(
                        "quality",
                        request,
                        body
                    );


                return false;
            }


            Console.WriteLine(
                $"[MES API] Quality Result 저장 성공 - ProductId={productId} / {message.Result}"
            );

            Console.WriteLine(
                $"[MES API] Response: {body}"
            );


            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[MES API] Quality Result 오류 - EventId={message.EventId}: {ex.Message}"
            );


            await _failedEventStore
                .SaveAsync(
                    "quality",
                    request,
                    ex.Message
                );


            return false;
        }
    }


    // =====================================================
    // Process Event 존재 확인
    //
    // Product History에서 같은 EventId가 있는지 확인
    // =====================================================

    private async Task<bool> ProcessEventExistsAsync(
        string productCode,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        string encodedProductCode =
            Uri.EscapeDataString(
                productCode
            );


        using var response =
            await SendWithRetryAsync(
                ct =>
                    _httpClient.GetAsync(
                        $"api/process/products/{encodedProductCode}/history",
                        ct
                    ),
                $"Process Event 확인/{productCode}",
                enableRetry: true,
                cancellationToken
            );


        if (response == null)
        {
            return false;
        }


        if (response.StatusCode ==
            HttpStatusCode.NotFound)
        {
            return false;
        }


        if (!response.IsSuccessStatusCode)
        {
            return false;
        }


        var events =
            await response.Content
                .ReadFromJsonAsync<
                    List<ProcessEventApiResponse>
                >(
                    cancellationToken:
                        cancellationToken
                );


        if (events == null)
        {
            return false;
        }


        return events.Any(
            x => x.EventId == eventId
        );
    }


    // =====================================================
    // Process Event POST
    //
    // EventId 기반 Idempotent Retry
    // =====================================================

    public async Task<bool> PostProcessEventAsync(
        long productId,
        string productCode,
        string toLocationCode,
        string eventType = "MOVE",
        DateTimeOffset? eventTime = null,
        string? remarks = null,
        Guid? eventId = null,
        CancellationToken cancellationToken = default)
    {
        var actualEventId =
            eventId
            ?? Guid.NewGuid();


        var request =
            new MoveProductApiRequest
            {
                EventId =
                    actualEventId,

                ProductId =
                    productId,

                ToLocationCode =
                    toLocationCode,

                EventType =
                    eventType,

                EventTime =
                    eventTime
                    ?? DateTimeOffset.UtcNow,

                SourceSystem =
                    "GEMINI",

                Remarks =
                    remarks
            };


        try
        {
            using var response =
                await SendWithRetryAsync(
                    ct =>
                        _httpClient.PostAsJsonAsync(
                            "api/process/move",
                            request,
                            ct
                        ),
                    $"Process/{productCode}/{toLocationCode}",
                    enableRetry: true,
                    cancellationToken
                );


            // -----------------------------------------
            // 응답 유실 가능성 검사
            // -----------------------------------------

            if (response == null)
            {
                if (await ProcessEventExistsAsync(
                    productCode,
                    actualEventId,
                    cancellationToken))
                {
                    Console.WriteLine(
                        $"[MES API] Process Event DB 저장 확인 완료 - {actualEventId}"
                    );


                    return true;
                }


                await _failedEventStore
                    .SaveAsync(
                        "process",
                        request,
                        "최종 HTTP 연결 실패 또는 Timeout"
                    );


                return false;
            }


            var body =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken
                    );


            // -----------------------------------------
            // HTTP 실패
            //
            // Duplicate EventId라면 DB에서 확인 후
            // 성공으로 간주
            // -----------------------------------------

            if (!response.IsSuccessStatusCode)
            {
                if (await ProcessEventExistsAsync(
                    productCode,
                    actualEventId,
                    cancellationToken))
                {
                    Console.WriteLine(
                        $"[MES API] Process Event 이미 DB에 존재 - {actualEventId}"
                    );


                    return true;
                }


                Console.WriteLine(
                    "[MES API] Process Event 저장 실패"
                );

                Console.WriteLine(
                    $"[MES API] EventId: {actualEventId}"
                );

                Console.WriteLine(
                    $"[MES API] ProductId: {productId}"
                );

                Console.WriteLine(
                    $"[MES API] ProductCode: {productCode}"
                );

                Console.WriteLine(
                    $"[MES API] ToLocation: {toLocationCode}"
                );

                Console.WriteLine(
                    $"[MES API] HTTP {(int)response.StatusCode} {response.StatusCode}"
                );

                Console.WriteLine(
                    $"[MES API] Response: {body}"
                );


                await _failedEventStore
                    .SaveAsync(
                        "process",
                        request,
                        body
                    );


                return false;
            }


            Console.WriteLine(
                $"[MES API] Process Event 저장 성공 - ProductId={productId} → {toLocationCode}"
            );

            Console.WriteLine(
                $"[MES API] Response: {body}"
            );


            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[MES API] Process Event 오류 - ProductId={productId}: {ex.Message}"
            );


            await _failedEventStore
                .SaveAsync(
                    "process",
                    request,
                    ex.Message
                );


            return false;
        }
    }
}