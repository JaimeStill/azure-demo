using System.Text.Json;
using System.Text.Json.Serialization;
using Arma.Demo.Core.Middleware;
using Arma.Demo.Core.Sync;
using Sync.Hubs;

var builder = WebApplication.CreateBuilder(args);

string[] origins = builder
    .Configuration
    .GetSection("CorsOrigins")
    .Get<string[]>()
?? new string[] {
    "http://localhost:4200",
    "https://jps-core-api.azurewebsites.net",
    "https://jps-processor.azurewebsites.net"
};

// Add services to the container.
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithOrigins(origins)
              .WithExposedHeaders(
                "Content-Disposition",
                "Access-Control-Allow-Origin"
              )
    )
);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddSyncGroupProvider();

var app = builder.Build();
app.UseJsonExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();
app.MapHubs();

app.Run();
