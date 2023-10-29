using Serilog;
using SimpleAPI.Infrastructure;
using SimpleAPI.Infrastructure.Endpoints;
using SimpleAPI.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

SerilogHelpers.SetLoggingPath();
builder.Host.UseSerilog((_, services, configuration) => configuration
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services.AddPersistenceServices();

var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.Run();
