using Microsoft.EntityFrameworkCore;
using Respawn;
using Xunit;

namespace Lewee.IntegrationTests;

/// <summary>
/// Database Context Fixture
/// </summary>
/// <typeparam name="T">Database context type</typeparam>
public abstract class DatabaseContextFixture<T> : IAsyncLifetime
    where T : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContextFixture{T}"/> class
    /// </summary>
    protected DatabaseContextFixture()
    {
    }

    /// <summary>
    /// Gets the database reset options
    /// </summary>
    protected abstract RespawnerOptions ResetOptions { get; }

    /// <summary>
    /// Gets or sets the database connection string
    /// </summary>
    protected string ConnectionString { get; set; }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        var respawner = await Respawner.CreateAsync(this.ConnectionString, this.ResetOptions);
        await respawner.ResetAsync(this.ConnectionString);
    }

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
