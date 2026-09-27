using Mes.Server.Database;
using Mes.Server.Repositories.Lot;
using Mes.Server.Repositories.Machine;
using Mes.Server.Repositories.Product;
using Mes.Server.Repositories.Quality;
using Mes.Server.Repositories.WorkOrder;
using Mes.Server.Repositories.Process;
using Mes.Server.Repositories.Alarm;
using Mes.Server.Services.WorkOrder;
using Mes.Server.Repositories.Production;
using Mes.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();

// DB ½Ì±ÛÅæ ¿¬°á
builder.Services.AddSingleton<MesDbConnectionFactory>();

// ±¸ÇöÃ¼ ³Ö±â
builder.Services.AddScoped<IMachineStatusRepository,MachineStatusRepository>();
builder.Services.AddScoped<IQualityResultRepository,QualityResultRepository>();
builder.Services.AddScoped<IWorkOrderRepository,WorkOrderRepository>();
builder.Services.AddScoped<ILotRepository,LotRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProcessRepository,ProcessRepository>();
builder.Services.AddScoped<IAlarmRepository,AlarmRepository>();
builder.Services.AddScoped<IWorkOrderService,WorkOrderService>();
builder.Services.AddScoped<IProductionRepository,ProductionRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapHub<GatewayHub>(
    "/hubs/gateway"
);

app.Run();