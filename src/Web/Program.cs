using SimpleAPI.Application;
using SimpleAPI.Infrastructure;
using SimpleAPI.Web.Setup;
using SimpleAPI.Web.Setup.Environment;

var environment = SimpleAPIEnvironment.Current();
environment.DisplayLogo();

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .PrepareHost(builder.Configuration);
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddHostServices(environment);

var app = builder.Build();
app.ConfigurePipeline(app.Environment);
app.MapEndpoints(builder.Configuration);

app.Run();

public partial class Program { }
