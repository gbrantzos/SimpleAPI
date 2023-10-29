using Serilog;
using SimpleAPI.Infrastructure;
using SimpleAPI.Infrastructure.Endpoints;
using SimpleAPI.Infrastructure.Persistence;
using SimpleAPI.Infrastructure.Setup;

var builder = WebApplication.CreateBuilder(args);

SerilogHelpers.SetLoggingPath();
builder.Host.UseSerilog((_, services, configuration) =>
{
    var requestContextEnricher = services.GetRequiredService<RequestContextEnricher>();
    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.With(requestContextEnricher);
});

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services
    .AddApplicationServices()
    .AddPersistenceServices();

var app = builder.Build();
app.UseRequestContext();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.Run();
