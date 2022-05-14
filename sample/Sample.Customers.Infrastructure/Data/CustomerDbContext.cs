using MediatR;
using Microsoft.EntityFrameworkCore;
using Saji.Infrastructure.Data;
using Sample.Customers.Domain.Entities;
using Serilog;

namespace Sample.Customers.Infrastructure.Data;

public sealed class CustomerDbContext : BaseApplicationDbContext<CustomerDbContext>
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options, IMediator mediator, ILogger logger)
        : base(options, mediator, logger)
    {
    }

    public override string Schema => "cus";

    public DbSet<Customer>? Customers { get; set; }

    protected override void ConfigureDatabaseModel(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
    }
}
