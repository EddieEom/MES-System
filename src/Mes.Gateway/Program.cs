using Mes.Gateway.Database;
using Mes.Gateway.Mqtt;
using Mes.Gateway.Routing;
using Mes.Gateway.Services;

Console.WriteLine(
    "================================="
);

Console.WriteLine(
    " MES Gateway Starting..."
);

Console.WriteLine(
    "================================="
);


// ======================================
// MSSQL Connection
// ======================================

string connectionString =
    "Server=localhost;" +
    "Database=MES_SYSTEM;" +
    "Integrated Security=True;" +
    "TrustServerCertificate=True;";

var dbConnectionFactory =
    new MesDbConnectionFactory(
        connectionString
    );


try
{
    await dbConnectionFactory
        .TestConnectionAsync();
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine(
        "[DB ERROR] MSSQL 연결 실패"
    );

    Console.WriteLine(
        $"[DB ERROR] {ex.Message}"
    );

    return;
}


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


// ======================================
// MQTT Router
// ======================================

var router =
    new MqttMessageRouter(
        machineStatusService,
        qualityService,
        productionService,
        dashboardService
    );


// ======================================
// MQTT Client
// ======================================

var mqttService =
    new MqttClientService(
        router
    );


try
{
    await mqttService.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine(
        $"[ERROR] Gateway 시작 실패: {ex.Message}"
    );

    return;
}


Console.WriteLine();
Console.WriteLine(
    "Gateway 실행 중..."
);

Console.WriteLine(
    "종료하려면 ENTER를 누르세요."
);


Console.ReadLine();


await mqttService.StopAsync();


Console.WriteLine(
    "MES Gateway 종료"
);