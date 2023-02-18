namespace Lewee.Infrastructure.AspNet.Settings;

/// <summary>
/// Application Settings
/// </summary>
public class ApplicationSettings
{
    /// <summary>
    /// Gets or sets the application name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the application environment
    /// </summary>
    public string Environment { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the application version
    /// </summary>
    public string Version { get; set; } = string.Empty;
}
