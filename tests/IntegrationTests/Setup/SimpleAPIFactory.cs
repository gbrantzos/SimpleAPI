using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleAPI.Infrastructure.Persistence;
using SimpleAPI.Web;

namespace SimpleAPI.IntegrationTests.Setup;


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
        builder.ConfigureServices(services =>
        {
            // Remove the app's database  registration.
            var serviceDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SimpleAPIContext>));
            
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }
            
            services.AddDbContext<SimpleAPIContext>(options =>
            {
                options.UseSqlite(_connection);
            });
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<SimpleAPIContext>();
            var logger = scopedServices.GetRequiredService<ILogger<SimpleAPIFactory>>();

            db.Database.EnsureCreated();
            try
            {
                //Utilities.InitializeDbForTests(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing test database: {Message}", ex.Message);
            }
        });
    }
    
    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public SimpleAPIContext GetContext()
    {
        var options = new DbContextOptionsBuilder<SimpleAPIContext>()
            .UseSqlite(_connection)
            .Options;

        return new SimpleAPIContext(options);
    }

    protected override void Dispose(bool disposing)
    {
        _connection.Close();
        base.Dispose(disposing);
    }
}
