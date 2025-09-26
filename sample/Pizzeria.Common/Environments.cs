namespace Pizzeria.Common;

public static class Environments
{
    public const string IsIntegrationTestEnvironmentVariableName = "IS_INTEGRATION_TEST";

    public const string IntegrationTesting = "IntegrationTesting";

    public static bool IsIntegrationTesting =>
        Environment.GetEnvironmentVariable(IsIntegrationTestEnvironmentVariableName) == "TRUE";

    public static void SetToIntegrationTesting()
    {
        Environment.SetEnvironmentVariable(IsIntegrationTestEnvironmentVariableName, "TRUE");
    }
}
