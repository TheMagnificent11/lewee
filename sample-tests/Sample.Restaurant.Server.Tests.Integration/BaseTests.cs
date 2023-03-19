using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample.Restaurant.Infrastructure.Data;
using Xunit;

namespace Sample.Restaurant.Server.Tests.Integration;

public abstract class BaseTests : IClassFixture<SampleRestaurantWebApplicationFactory>, IDisposable
{
    private readonly SampleRestaurantWebApplicationFactory factory;

    private bool disposedValue;

    protected BaseTests(SampleRestaurantWebApplicationFactory factory)
    {
        this.factory = factory;

        this.HttpClient = factory.CreateClient();

        var scopeFactory = this.GetServiceScopeFactory();

        using (var scope = scopeFactory.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<RestaurantDbContext>();

            // Ensure we have a freshly created database for each test
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }

    protected HttpClient HttpClient { get; }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected static HttpRequestMessage GetHttpRequestMessage(HttpMethod httpMethod, string apiPath, object content)
    {
        var request = new HttpRequestMessage(httpMethod, apiPath)
        {
            Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
        };

        return request;
    }

    protected static async Task<T> DeserializeResponse<T>(HttpResponseMessage response, bool isSuccess = true)
    {
        if (isSuccess)
        {
            response.EnsureSuccessStatusCode();
        }

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

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

    protected async Task<T> HttpGet<T>(string apiPath)
    {
        using (var request = new HttpRequestMessage(HttpMethod.Get, apiPath))
        {
            using (var response = await this.HttpClient.SendAsync(request))
            {
                return await DeserializeResponse<T>(response);
            }
        }
    }

    protected async Task<List<T>> GetData<T>(Expression<Func<T, bool>> predicate)
        where T : class
    {
        var scopeFactory = this.GetServiceScopeFactory();

        using (var scope = scopeFactory.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;

            var dbContext = scopedServices.GetRequiredService<RestaurantDbContext>();

            return await dbContext
                .Set<T>()
                .Where(predicate)
                .ToListAsync();
        }
    }

    private IServiceScopeFactory GetServiceScopeFactory()
    {
        var scopeFactory = this.factory.Services.GetService<IServiceScopeFactory>()
            ?? throw new InvalidOperationException("Could not get scope factory");

        return scopeFactory;
    }
}
