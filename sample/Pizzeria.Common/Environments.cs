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

        public static class DefaultAdminCredentialsForTesting
        {
            public const string Username = "admin";
            public const string Password = "!MySuperStrongPassword123!";
        }

        public static class Users
        {
            public static class Customer1
            {
                public const string Username = "customer";
                public const string Password = "Password123!";
            }

            public static class FrontStaff1
            {
                public const string Username = "front-staff";
                public const string Password = "Password123!";
            }

            public static class KitchenStaff1
            {
                public const string Username = "kitchen-staff";
                public const string Password = "Password123!";
            }

            public static class DeliveryDriver1
            {
                public const string Username = "delivery-driver";
                public const string Password = "Password123!";
            }
        }
    }
}
