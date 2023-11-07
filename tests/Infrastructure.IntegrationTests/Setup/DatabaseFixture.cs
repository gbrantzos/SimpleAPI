using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimpleAPI.Infrastructure.Persistence;

namespace SimpleAPI.Infrastructure.IntegrationTests.Setup;

// ReSharper disable once ClassNeverInstantiated.Global
public class DatabaseFixture : IDisposable
{
    private readonly SqliteConnection? _connection;
    private readonly IConfigurationRoot _configuration;

    public SimpleAPIContext Context { get; }

    public DatabaseFixture()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables(); 
        _configuration = configurationBuilder.Build();
        
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        Context = PrepareContext();
    }

    private SimpleAPIContext PrepareContext()
    {
        var connectionString = _configuration.GetConnectionString("SimpleAPI_Tests");
        var options = new DbContextOptionsBuilder<SimpleAPIContext>()
            //.UseSqlite(_connection)
            .UseSqlServer(connectionString)
            .Options;
        var context = new SimpleAPIContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        _connection?.Dispose();
    }
}
