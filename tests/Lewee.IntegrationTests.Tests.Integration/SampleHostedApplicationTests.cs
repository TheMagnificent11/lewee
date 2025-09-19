using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Lewee.IntegrationTests.Tests.Integration;

public class SampleHostedApplicationTests : HostedApplicationTests
{
    private const string SampleApiPath = "/api/sample";

    private IConfiguration configuration;
    private Mock<ISampleDataContext> sampleDataContextMock;

    [Fact]
    public async Task HttpClientCanBeUsedToCallHostedApplication()
    {
        using (var httpClient = this.GetTestHttpClient())
        {
            var response = await httpClient.PostAsync(SampleApiPath, new StringContent(string.Empty));

            response.EnsureSuccessStatusCode();

            this.sampleDataContextMock?.Verify(x => x.SaveChanges(), Times.Once);
        }
    }

    protected override void SetupAppConfiguration(IConfigurationBuilder configurationBuilder)
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            ["Data:Server"] = "localhost",
            ["Data:UserId"] = "sa",
            ["Data:Password"] = "password"
        };

        configurationBuilder.AddInMemoryCollection(inMemorySettings!);

        this.configuration = configurationBuilder.Build();
    }

    protected override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        this.sampleDataContextMock = new();

        services.AddSampleDataContextConfiguration(this.configuration!);
        services.AddSingleton(_ => this.sampleDataContextMock.Object);
    }

    protected override void ConfigureApplication(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapPost(SampleApiPath, async context =>
            {
                var sampleDataContext = context.RequestServices.GetRequiredService<ISampleDataContext>();

                await sampleDataContext.SaveChanges();

                context.Response.StatusCode = 200;
            });
        });
    }
}
