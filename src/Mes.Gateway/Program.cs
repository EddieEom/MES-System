using Mes.Gateway.Api;
using Mes.Gateway.Mqtt;
using Mes.Gateway.Routing;
using Mes.Gateway.Services;


// ======================================
// MES Gateway Start
// ======================================

Console.WriteLine(
    "================================="
);

Console.WriteLine(
    " MES 게이트웨이 여는 중..."
);

Console.WriteLine(
    "================================="
);

Console.WriteLine();


// ======================================
// Mes.Server HTTP Client
// ======================================

// 개발 환경 기본 주소
// 필요하면 환경변수 MES_SERVER_BASE_URL로 변경 가능
string mesServerBaseUrl =
    Environment.GetEnvironmentVariable(
        "MES_SERVER_BASE_URL"
    )
    ?? "https://localhost:7075/";


var httpClient =
    new HttpClient
    {
        BaseAddress =
            new Uri(
                mesServerBaseUrl
            ),

        // Timeout은 MesApiClient에서
        // 요청 단위로 관리
        Timeout =
            Timeout.InfiniteTimeSpan
    };


var failedEventStore =
    new FailedEventStore();


var mesApiClient =
    new MesApiClient(
        httpClient,
        failedEventStore
    );


// ======================================
// Mes.Server Connection Test
// ======================================

Console.WriteLine(
    "[Gateway] Mes.Server 연결 확인 중..."
);

Console.WriteLine(
    $"[Gateway] Server: {mesServerBaseUrl}"
);

Console.WriteLine();


bool serverConnected =
    await mesApiClient
        .CheckConnectionAsync();


if (!serverConnected)
{
    Console.WriteLine();

    Console.WriteLine(
        "[Gateway] Mes.Server 연결 실패"
    );

    Console.WriteLine(
        "[Gateway] Gateway를 종료합니다."
    );

    httpClient.Dispose();

    return;
}


Console.WriteLine();

Console.WriteLine(
    "[Gateway] Mes.Server 연결 확인 완료"
);

Console.WriteLine();


// ======================================
// MES Services
// ======================================

var machineStatusService =
    new MachineStatusService();


var qualityService =
    new QualityService();


var productionService =
    new ProductionService(
        qualityService
    );


var dashboardService =
    new MesDashboardService(
        machineStatusService,
        productionService
    );

var activeProductService =
    new ActiveProductService();


// ======================================
// MQTT Router
// ======================================

var router =
    new MqttMessageRouter(
        machineStatusService,
        qualityService,
        productionService,
        dashboardService,
        activeProductService,
        mesApiClient
        
    );


// ======================================
// MQTT Client
// ======================================

var mqttService =
    new MqttClientService(
        router
    );


// ======================================
// MQTT Start
// ======================================

try
{
    await mqttService
        .StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine();

    Console.WriteLine(
        $"[ERROR] Gateway 시작 실패: {ex.Message}"
    );

    httpClient.Dispose();

    return;
}


// ======================================
// Running
// ======================================

Console.WriteLine();

Console.WriteLine(
    "================================="
);

Console.WriteLine(
    " MES Gateway Running"
);

Console.WriteLine(
    "================================="
);

Console.WriteLine();

Console.WriteLine(
    "종료하려면 ENTER를 누르세요."
);


Console.ReadLine();


// ======================================
// MQTT Stop
// ======================================

try
{
    await mqttService
        .StopAsync();
}
catch (Exception ex)
{
    Console.WriteLine(
        $"[ERROR] MQTT 종료 중 오류: {ex.Message}"
    );
}


// ======================================
// HTTP Client Dispose
// ======================================

httpClient.Dispose();


Console.WriteLine();

Console.WriteLine(
    "MES Gateway 종료"
);