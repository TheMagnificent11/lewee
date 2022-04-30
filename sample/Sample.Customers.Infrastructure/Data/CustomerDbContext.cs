using Microsoft.EntityFrameworkCore;
using Saji.Infrastructure.Data;

namespace Sample.Customers.Infrastructure.Data;

public sealed class CustomerDbContext : BaseApplicationDbContext<CustomerDbContext>
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    /* TODO: public override string Schema => "cus"; */

    protected override void ConfigureDatabaseModel(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
    }
}
