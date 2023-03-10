using Arma.Demo.Core.Middleware;
using Arma.Demo.Core.Sync;
using SyncServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

string[] origins = builder
    .Configuration
    .GetSection("CorsOrigins")
    .Get<string[]>()
?? new string[] {
    "http://localhost:4200",
    "http://localhost:5000",
    "http://localhost:5001"
};

// Add services to the container.
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithOrigins(origins)
    )
);

builder.Services.AddSignalR();
builder.Services.AddSyncServiceManager();

var app = builder.Build();

app.Services.RegisterSyncServices(new List<SyncService>()
{
    new("Processor", "http://localhost:5000/api/ping", typeof(ProcessorHub))
});

await app.Services.InitializeSyncServiceManager();

app.UseJsonExceptionHandler();
app.UseCors();
app.MapHubs();

app.Run();