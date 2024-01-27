using Sample.Restaurant.Data;

namespace Sample.Restaurant.SqlServer;

public sealed class SqlServerRestaurantDbSeeder : RestaurantDbSeeder<SqlServerRestaurantDbContext>
{
    public SqlServerRestaurantDbSeeder(SqlServerRestaurantDbContext context)
        : base(context)
    {
    }
}
