namespace Pizzeria.Common;

public static class ServiceNames
{
    public const string DatabaseServer = "database-server";

    public const string PizzaStoreDatabase = "pizza-store-database";

    public const string PizzaStoreDatabaseIntegrationTesting = "pizza-store-integration-testing-database";

    public const string PizzaStoreApi = "pizza-store-api";

    public static string GetPizzaStoreDatabaseName()
    {
        if (Environments.IsIntegrationTest)
        {
            return PizzaStoreDatabaseIntegrationTesting;
        }
        return PizzaStoreDatabase;
    }
}
