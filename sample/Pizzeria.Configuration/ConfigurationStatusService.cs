namespace Pizzeria.Configuration;

internal sealed class ConfigurationStatusService
{
    public bool IsConfigurationComplete { get; private set; }

    public bool ConfigurationFailed { get; private set; }

    public void SetConfigurationComplete()
    {
        this.IsConfigurationComplete = true;
    }

    public void SetConfigurationFailed()
    {
        this.ConfigurationFailed = true;
    }
}
