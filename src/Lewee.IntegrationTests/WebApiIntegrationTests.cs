using System.Data;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lewee.IntegrationTests;

/// <summary>
/// Web API Integration Tests
/// </summary>
/// <typeparam name="TEntryPoint">ASP.Net app entrypoint class</typeparam>
/// <typeparam name="TFactory">Web application factory type</typeparam>
public abstract class WebApiIntegrationTests<TEntryPoint, TFactory> : IClassFixture<TFactory>
    where TEntryPoint : class
    where TFactory : WebApplicationFactory<TEntryPoint>
{
    private readonly TFactory factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiIntegrationTests{TEntryPoint, TFactory}"/> class
    /// </summary>
    /// <param name="factory">Web application factory</param>
    protected WebApiIntegrationTests(TFactory factory)
    {
        this.factory = factory;
    }

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
        // TODO (https://github.com/TheMagnificent11/lewee/issues/14): add request body parameter
        using (var request = new HttpRequestMessage(method, apiPath))
        using (var httpClient = this.factory.CreateClient())
        {
            return await httpClient.SendAsync(request);
        }
    }

    /// <summary>
    /// Calls the health check endpoint and ensures a success status code
    /// </summary>
    /// <param name="healthPath">Health path</param>
    /// <returns>An asynchronous task</returns>
    protected async Task HealthCheck(string healthPath)
    {
        using (var response = await this.HttpRequest(HttpMethod.Get, healthPath))
        {
            response.EnsureSuccessStatusCode();
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

        return await response.Content.ReadFromJsonAsync<T>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
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
