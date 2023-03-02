using Arma.Demo.Core.Middleware;
using Processor.Hubs;
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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddSingleton<HubGroupProvider>();
builder.Services.AddSingleton<ProcessorService>();
builder.Services.AddSingleton<PingService>();

var app = builder.Build();

app.UseJsonExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseAuthorization();

app.MapControllers();
app.MapHubs();

app.Run();
