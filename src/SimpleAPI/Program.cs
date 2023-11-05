using SimpleAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .PrepareHost(builder.Configuration);
builder.Services
    .AddSystemServices()
    .AddApplicationServices()
    .AddInfrastructure();

var app = builder.Build();
app.ConfigurePipeline(app.Environment);
app.MapEndpoints();

app.Run();
