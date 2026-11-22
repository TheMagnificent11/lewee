using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Lewee.Auth.Api;
using Lewee.Infrastructure.Refit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Moq;
using Pizzeria.Ordering.StateManagement;
using Xunit;

namespace Pizzeria.Ordering.Web.Tests.Unit;

public sealed class BffApiClientTests
{
    [Fact]
    public async Task Should_SerializeRequestsAndDeserializeResponses_When_ClientIsRegistered()
    {
        using var handler = new TestHttpMessageHandler();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWebApiHttpClient<IBffApiClient>("bff");
        services.ConfigureAll<HttpClientFactoryOptions>(
            options => options.HttpMessageHandlerBuilderActions.Add(builder => builder.PrimaryHandler = handler));
        var authentication = new Mock<IAuthenticationService>();
        authentication
            .Setup(service => service.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(AuthenticateResult.NoResult());
        services.AddSingleton(authentication.Object);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IHttpContextAccessor>().HttpContext = new DefaultHttpContext
        {
            RequestServices = provider,
        };
        var client = provider.GetRequiredService<IBffApiClient>();

        await client.CreateUserAsync(new CreateUserRequest { ExternalUserId = "external-user" });
        var pizzas = await client.GetPizzasAsync();

        using var requestJson = JsonDocument.Parse(handler.RequestBody!);
        requestJson.RootElement.GetProperty("externalUserId").GetString().Should().Be("external-user");
        pizzas.Should().ContainSingle().Which.Name.Should().Be("Margherita");
    }

    private sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        public string? RequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Content is not null)
            {
                this.RequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            var response = request.Method == HttpMethod.Get
                ? """[{"id":"00000000-0000-0000-0000-000000000001","name":"Margherita","description":"Classic","price":10}]"""
                : string.Empty;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response, Encoding.UTF8, "application/json"),
            };
        }
    }
}
