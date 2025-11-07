namespace Pizzeria.Configuration;

internal sealed class ConfigurationStatusService
{
    private bool configurationComplete;
    private bool configurationFailed;

    public bool IsConfigurationComplete => this.configurationComplete;

    public bool ConfigurationFailed => this.configurationFailed;

    public void SetConfigurationComplete()
    {
        this.configurationComplete = true;
    }

    public void SetConfigurationFailed()
    {
        this.configurationFailed = true;
    }
}
