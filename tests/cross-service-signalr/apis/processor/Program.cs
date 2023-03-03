using Arma.Demo.Core.Middleware;
using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;
using Processor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
    )
);

builder.Services.AddSyncService<ProcessorService, Package>();

var app = builder.Build();

await ProcessorService.Initialize(app.Services);

app.UseJsonExceptionHandler();
app.UseCors();
app.Run();
