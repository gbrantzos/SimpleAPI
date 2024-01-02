using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Time.Testing;
using Moq;
using SimpleAPI.Application.Common;
using SimpleAPI.Infrastructure.Persistence;
using Testcontainers.MySql;

namespace SimpleAPI.IntegrationTests.Setup;

// ReSharper disable once ClassNeverInstantiated.Global
public class DatabaseFixture : IAsyncLifetime
{
    private readonly FakeTimeProvider _fakeTimeProvider = new ();
    private readonly IContainer? _container;
    private SimpleAPIContext? _context;
    private string? _connectionString;

    public SimpleAPIContext Context => _context ??
        throw new InvalidOperationException($"{nameof(DatabaseFixture)} not initialized");

    public string ConnectionString => _connectionString ??
        throw new InvalidOperationException($"{nameof(DatabaseFixture)} not initialized");

    public FakeTimeProvider FakeTimeProvider => _fakeTimeProvider;

    public DatabaseFixture()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        var configuration = configurationBuilder.Build();

        if (configuration.GetValue<bool>("PrepareContainer"))
        {
            _container = new MySqlBuilder()
                .WithImage(configuration.GetValue<string>("TestContainer:Image"))
                .WithName($"SimpleAPI_Tests_{Guid.NewGuid():N}")
                .WithPortBinding(MySqlBuilder.MySqlPort, true)
                .WithEnvironment("MYSQL_ROOT_PASSWORD", "secret")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MySqlBuilder.MySqlPort))
                .Build();
        }
        else
        {
            _connectionString = configuration.GetConnectionString("SimpleAPI_Tests") ??
                throw new InvalidOperationException("Connection string for tests is not defined [SimpleAPI_Tests]");
        }
    }

    private void PrepareContext()
    {
        var options = new DbContextOptionsBuilder<SimpleAPIContext>()
            .UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString))
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;
        _context = new SimpleAPIContext(options, _fakeTimeProvider);
        _context.Database.EnsureCreated();
    }

    public async Task InitializeAsync()
    {
        if (_container is not null)
        {
            await _container.StartAsync();
            var port = _container.GetMappedPublicPort(MySqlBuilder.MySqlPort);
            _connectionString = $"server=localhost;user=root;password=secret;database=simple_api_tests;port={port}";
        }
        PrepareContext();
    }

    public async Task DisposeAsync()
    {
        await _context!.Database.EnsureDeletedAsync();
        if (_container is not null)
            await _container.StopAsync();
    }
}
