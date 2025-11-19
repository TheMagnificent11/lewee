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
        public const string RealmName = "lewee-pizzeria";

        public static class Clients
        {
            public const string StoreApi = "pizzeria-store-api";
            public const string StoreWeb = "pizzeria-store-web";
        }

        public static class DefaultAdminCredentialsForTesting
        {
            public const string Username = "admin";
            public const string Password = "!MySuperStrongPassword123!";
        }
    }
}
