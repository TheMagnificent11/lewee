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

        public const string ApiClientId = "pizzeria-store-api";

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

        public static class Users
        {
            public static class Customer1
            {
                public const string Username = "customer-1";
                public const string Password = "Password123!";
            }

            public static class FrontStaff1
            {
                public const string Username = "front-staff-1";
                public const string Password = "Password123!";
            }

            public static class KitchenStaff1
            {
                public const string Username = "kitchen-staff-1";
                public const string Password = "Password123!";
            }

            public static class DeliveryDriver1
            {
                public const string Username = "delivery-driver-1";
                public const string Password = "Password123!";
            }
        }
    }
}
