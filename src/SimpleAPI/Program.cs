using Serilog;
using SimpleAPI.Infrastructure;
using SimpleAPI.Infrastructure.Setup;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

SerilogHelpers.SetLoggingPath();
builder.Host
    .AddAutofac()
    .AddSerilog(builder.Configuration);

builder.Services
    .AddEndpointsApiExplorer()
    .AddProblemDetails(options => options.CustomizeProblemDetails = ProblemDetailsHelpers.CustomizeProblemDetails)
    .AddSwaggerGen();

builder.Services
    .AddApplicationServices()
    .AddPersistenceServices();

var app = builder.Build();
app.UseRequestContext();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocExpansion(DocExpansion.List);
        options.DefaultModelsExpandDepth(-1);
    });
}

app.MapEndpoints();

app.Run();
