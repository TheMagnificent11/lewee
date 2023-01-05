using Microsoft.Extensions.Configuration;

namespace Lewee.Infrastructure.Settings;

/// <summary>
/// Settings Extension Methods
/// </summary>
public static class SettingsExtensions
{
    /// <summary>
    /// Get Settings
    /// </summary>
    /// <typeparam name="T">
    /// Type to which to bind settings in <paramref name="configurationSection"/>
    /// </typeparam>
    /// <param name="configuration">
    /// Configuration
    /// </param>
    /// <param name="configurationSection">
    /// Configuration section
    /// </param>
    /// <returns>
    /// Instance of <typeparamref name="T"/>
    /// </returns>
    public static T GetSettings<T>(this IConfiguration configuration, string configurationSection)
        where T : class, new()
    {
        var settings = new T();

        configuration.Bind(configurationSection, settings);

        return settings;
    }
}
