namespace Pizzeria.Common;

public static class ServiceNames
{
    public const string DatabaseServer = "database-server";

    public const string PizzaStoreApi = "pizza-store-api";

    public const string AuthApi = "auth-api";

    public const string Bff = "bff";

    public const string PizzaStoreWeb = "pizza-store-web";

    public const string AuthServer = "auth-server";

    public const string ConfigurationService = "configuration-service";

    public static string PizzeriaDatabase => Environments.IsIntegrationTesting
        ? "pizzeria-integration-testing-database"
        : "pizzeria-database";
}
