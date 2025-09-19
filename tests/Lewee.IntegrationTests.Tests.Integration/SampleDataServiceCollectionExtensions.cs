using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.IntegrationTests.Tests.Integration;

internal static class SampleDataServiceCollectionExtensions
{
    public static IServiceCollection AddSampleDataContextConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var server = configuration["Data:Server"];
        var userId = configuration["Data:UserId"];
        var password = configuration["Data:Password"];

        if (string.IsNullOrWhiteSpace(server) ||
            string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("'Data' app settings not correctly configured");
        }

        services.AddSingleton(new SampleDataConfiguration
        {
            Server = server,
            UserId = userId,
            Password = password,
        });

        return services;
    }
}
