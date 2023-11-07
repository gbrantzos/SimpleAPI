using SimpleAPI.Infrastructure;

var environment = SimpleAPIEnvironment.Current();
environment.DisplayLogo();

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .PrepareHost(builder.Configuration);
builder.Services
    .AddSystemServices()
    .AddApplicationServices(environment)
    .AddInfrastructure();

var app = builder.Build();
app.ConfigurePipeline(app.Environment);
app.MapEndpoints(builder.Configuration);

app.Run();
