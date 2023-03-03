using Arma.Demo.Core.Middleware;
using Arma.Demo.Core.Sync;
using SyncServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
    )
);

builder.Services.AddSignalR();
builder.Services.AddSyncGroupProvider();

var app = builder.Build();
app.UseJsonExceptionHandler();
app.UseCors();
app.MapHubs();

app.Run();
