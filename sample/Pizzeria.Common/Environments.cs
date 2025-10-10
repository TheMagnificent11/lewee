namespace Pizzeria.Common;

public static class Environments
{
    public const string IsIntegrationTestEnvironmentVariableName = "IS_INTEGRATION_TEST";

    public const string IntegrationTesting = "IntegrationTesting";

    public const string AuthenticationSchemesBearerValidAudience = "Authentication__Schemes__Bearer__ValidAudience";

    public static bool IsIntegrationTesting =>
        string.Equals(Environment.GetEnvironmentVariable(IsIntegrationTestEnvironmentVariableName), "TRUE", StringComparison.Ordinal);

    public static void SetToIntegrationTesting()
    {
        Environment.SetEnvironmentVariable(IsIntegrationTestEnvironmentVariableName, "TRUE");
    }

    public static class Auth
    {
        public const string RealmName = "pizzeria";

        public const string ApiClientId = "pizzeria-store-api";

        public static class IntegrationTesting
        {
            public const string AdminUsername = "admin";
            public const string AdminPassword = "admin";
        }
    }
}
