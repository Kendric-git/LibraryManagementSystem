using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace LibraryManagementSystem.IntegrationTests.Infrastructure;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder("postgres:17-alpine")
            .WithDatabase("library_integration_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var context = CreateDbContext();

        await context.Database.MigrateAsync();
    }

    public LibrarySystemContext CreateDbContext()
    {
        var options =
            new DbContextOptionsBuilder<LibrarySystemContext>()
                .UseNpgsql(_container.GetConnectionString())
                .Options;

        return new LibrarySystemContext(options);
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = CreateDbContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}