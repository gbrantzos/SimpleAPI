using SimpleAPI.Infrastructure.Endpoints;
using SimpleAPI.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services.AddPersistenceServices();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapEndpoints();
app.Run();

public partial class Program { }
