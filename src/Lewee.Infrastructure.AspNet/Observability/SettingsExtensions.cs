using Microsoft.Extensions.Configuration;

namespace Lewee.Infrastructure.AspNet.Observability;

internal static class SettingsExtensions
{
    public static T GetSettings<T>(this IConfiguration configuration, string configurationSection)
        where T : class, new()
    {
        var settings = new T();

        configuration.Bind(configurationSection, settings);

        return settings;
    }
}
