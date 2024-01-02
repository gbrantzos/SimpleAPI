using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleAPI.Application.Common;
using SimpleAPI.Infrastructure.Persistence;

namespace SimpleAPI.FunctionalTests.Setup;

// ReSharper disable once ClassNeverInstantiated.Global
public class SimpleAPIFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public SimpleAPIFactory()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Change the app's database registration.
            services.RemoveAll(typeof(DbContextOptions<SimpleAPIContext>));
            services.AddDbContext<SimpleAPIContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var logger = scopedServices.GetRequiredService<ILogger<SimpleAPIFactory>>();
        var context = scopedServices.GetRequiredService<SimpleAPIContext>();

        context.Database.EnsureCreated();
        try
        {
            //Utilities.InitializeDbForTests(db);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing test database: {Message}", ex.Message);
        }

        return host;
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public SimpleAPIContext GetSqliteContext()
    {
        var options = new DbContextOptionsBuilder<SimpleAPIContext>()
            .UseSqlite(_connection)
            .Options;

        return new SimpleAPIContext(options, TimeProvider.System);
    }

    protected override void Dispose(bool disposing)
    {
        _connection.Close();
        base.Dispose(disposing);
    }
}
