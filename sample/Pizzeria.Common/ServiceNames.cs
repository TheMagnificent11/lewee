namespace Pizzeria.Common;

public static class ServiceNames
{
    public const string DatabaseServer = "database-server";

    public const string PizzaStoreDatabase = "pizza-store-database";

    public const string PizzaStoreDatabaseIntegrationTesting = "pizza-store-integration-testing-database";

    public const string PizzaStore = "pizza-store";

    public const string AuthServer = "auth-server";

    public const string ConfigurationService = "configuration-service";

    public static string PizzaStoreDatabaseName => Environments.IsIntegrationTesting
        ? PizzaStoreDatabaseIntegrationTesting
        : PizzaStoreDatabase;
}
