using SimpleAPI.Infrastructure;
using SimpleAPI.Infrastructure.Setup;

SerilogHelpers.SetLoggingPath();

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .AddAutofac()
    .AddSerilog(builder.Configuration);
builder.Services
    .AddSystemServices()
    .AddApplicationServices();

var app = builder.Build();

app.ConfigurePipeline(app.Environment);
app.MapEndpoints();

app.Run();
