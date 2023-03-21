using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Xunit;

namespace Lewee.IntegrationTests;

/// <summary>
/// Base Integration Tests
/// </summary>
/// <typeparam name="TEntryPoint">ASP.Net app entrypoint class</typeparam>
/// <typeparam name="TFactory">Web application factory type</typeparam>
public abstract class WebApiIntegrationTestsBase<TEntryPoint, TFactory> : IClassFixture<TFactory>, IDisposable
    where TEntryPoint : class
    where TFactory : WebApplicationFactory<TEntryPoint>
{
    private readonly TFactory factory;

    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiIntegrationTestsBase{TEntryPoint, TFactory}"/> class
    /// </summary>
    /// <param name="factory">Web application factory</param>
    protected WebApiIntegrationTestsBase(TFactory factory)
    {
        this.factory = factory;

        this.HttpClient = factory.CreateClient();
    }

    /// <summary>
    /// Gets the HTTP client to use to execute tests on the target Web API
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <summary>
    /// Gets the scope factory
    /// </summary>
    protected IServiceScopeFactory ScopeFactory
    {
        get
        {
            var scopeFactory = this.factory.Services.GetService<IServiceScopeFactory>()
                ?? throw new InvalidOperationException("Could not get scope factory");

            return scopeFactory;
        }
    }

    /// <summary>
    /// Gets an array of the test database detials
    /// </summary>
    protected abstract DatabaseResetConfiguration[] TestDatabases { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Creates a HTTP request message for the specified <paramref name="httpMethod"/> and <paramref name="apiPath"/>
    /// with <paramref name="content"/> has the request body
    /// </summary>
    /// <param name="httpMethod">HTTP method</param>
    /// <param name="apiPath">API path</param>
    /// <param name="content">Request body content</param>
    /// <returns>A HTTP request meesage</returns>
    protected static HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, string apiPath, object content)
    {
        var request = new HttpRequestMessage(httpMethod, apiPath)
        {
            Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
        };

        return request;
    }

    /// <summary>
    /// Disposes this class
    /// </summary>
    /// <param name="disposing">Whether disposal is currently executing further up the stack</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.HttpClient.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            this.disposedValue = true;
        }
    }

    /// <summary>
    /// Resets the database to the state defined in <see cref="DatabaseResetConfiguration"/>
    /// </summary>
    /// <typeparam name="T">Database context type</typeparam>
    /// <returns>Asynchronous task</returns>
    protected async Task ResetDatabase<T>()
        where T : DbContext
    {
        var dbDetails = this.TestDatabases.FirstOrDefault(x => x.DbContextType == typeof(T))
            ?? throw new InvalidOperationException(
                $"{typeof(T).FullName} not configured in {nameof(this.TestDatabases)}");

        using (var scope = this.ScopeFactory.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;

            if (!dbDetails.DbContextType.IsSubclassOf(typeof(DbContext)))
            {
                throw new InvalidOperationException(
                    $"{dbDetails.DbContextType.FullName} does not inherit from {typeof(DbContext).FullName}");
            }

            if (scopedServices.GetRequiredService(dbDetails.DbContextType) is not DbContext db)
            {
                throw new InvalidOperationException(
                    $"Could not cast {dbDetails.DbContextType.FullName} to {typeof(DbContext).FullName}");
            }

            var connectionString = db.Database.GetConnectionString()
                ?? throw new InvalidOperationException(
                    $"Could not get connection string for {dbDetails.DbContextType.FullName}");

            var respawner = await Respawner.CreateAsync(connectionString, dbDetails.RespawnerOptions);
            await respawner.ResetAsync(connectionString);
        }
    }

    /// <summary>
    /// Executes a HTTP GET request
    /// </summary>
    /// <typeparam name="T">Expected HTTP GET response type</typeparam>
    /// <param name="apiPath">API path</param>
    /// <returns>The deserialized body from the response to the HTTP GET request</returns>
    protected async Task<T?> HttpGet<T>(string apiPath)
    {
        using (var response = await this.HttpRequest(HttpMethod.Get, apiPath))
        {
            return await this.DeserializeResponse<T>(response);
        }
    }

    /// <summary>
    /// Executes a HTTP request
    /// </summary>
    /// <param name="method">HTTP method</param>
    /// <param name="apiPath">API path</param>
    /// <returns>The HTTP response</returns>
    protected async Task<HttpResponseMessage> HttpRequest(HttpMethod method, string apiPath)
    {
        using (var request = new HttpRequestMessage(method, apiPath))
        {
            return await this.HttpClient.SendAsync(request);
        }
    }

    /// <summary>
    /// Deserializes a HTTP response
    /// </summary>
    /// <typeparam name="T">Reponse body type</typeparam>
    /// <param name="response">Response to deserialize</param>
    /// <param name="isSuccess">Whether the response was expected successful</param>
    /// <returns>The deserialized data</returns>
    protected virtual async Task<T?> DeserializeResponse<T>(HttpResponseMessage response, bool isSuccess = true)
    {
        if (isSuccess)
        {
            response.EnsureSuccessStatusCode();
        }

        var json = await response.Content.ReadAsStringAsync();
        if (json == null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// Waits three seconds for domain events to be dispatched
    /// </summary>
    /// <returns>Asynchronous task</returns>
    protected async Task WaitForDomainEventsToBeDispatched()
    {
        await Task.Delay(TimeSpan.FromSeconds(3));
    }

    /// <summary>
    /// Reads data from the targeted <typeparamref name="TDbContext"/>
    /// </summary>
    /// <typeparam name="TData">DB context set type</typeparam>
    /// <typeparam name="TDbContext">DB context type</typeparam>
    /// <param name="predicate">Predicate for query</param>
    /// <returns>A list of matched data objects</returns>
    protected async Task<List<TData>> GetData<TData, TDbContext>(Expression<Func<TData, bool>> predicate)
        where TData : class
        where TDbContext : DbContext
    {
        using (var scope = this.ScopeFactory.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;

            var dbContext = scopedServices.GetRequiredService<TDbContext>();
            if (dbContext == null)
            {
                return new List<TData>();
            }

            return await dbContext
                .Set<TData>()
                .Where(predicate)
                .ToListAsync();
        }
    }
}
