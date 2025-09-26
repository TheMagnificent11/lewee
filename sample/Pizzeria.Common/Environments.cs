namespace Pizzeria.Common;

public static class Environments
{
    public const string IntegrationTesting = "IntegrationTesting";

    public static bool IsIntegrationTesting => Environment.GetEnvironmentVariable("IS_INTEGRATION_TEST") == "TRUE";
}