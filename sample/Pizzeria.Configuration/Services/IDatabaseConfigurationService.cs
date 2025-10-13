namespace Pizzeria.Configuration.Services;

public interface IDatabaseConfigurationService
{
    Task MigrateAsync();

    Task SeedDataAsync();
}
