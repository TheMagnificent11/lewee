namespace Pizzeria.Common;

public static class Environments
{
    public const string IntegrationTesting = "IntegrationTesting";

    public static bool IsIntegrationTest => Environment.GetEnvironmentVariable("IS_INTEGRATION_TEST") == "TRUE";
}