namespace Pizzeria.Common;

public static class ServiceNames
{
    public const string DatabaseServer = "database-server";

    public const string PizzaStoreDatabase = "pizza-store-database";

    public const string PizzaStoreDatabaseIntegrationTesting = "pizza-store-integration-testing-database";

    public const string PizzaStoreWeb = "pizza-store-web";

    public const string AuthServer = "auth-server";

    public const string ConfigurationService = "configuration-service";

    public const string SignalR = "signalr";

    public static string PizzaStoreDatabaseName => Environments.IsIntegrationTesting
        ? PizzaStoreDatabaseIntegrationTesting
        : PizzaStoreDatabase;
}
