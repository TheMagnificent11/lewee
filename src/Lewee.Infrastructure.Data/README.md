# Lewee.Infrastructure.Data

## Purpose

This project contains the data access layer for applications using `Lewee` dependencies.

## Dependencies

- `Lewee.Domain`
- `Ardalis.Specification.EntityFrameworkCore`
- `Mediatr`
- `Microsoft.Extensions.Configuration.Binder`
- `Microsoft.Extensions.DependencyModel`
- `Microsoft.Extensions.Hosting.Abstractions`
- `Serilog`

## Components

- Various classes to help configure Entity Framework entities
  - `AggregateRootConfiguration<T>`
  - `DomainEventReferenceConfiguration`
  - `EntityConfiguration<T>`
  - `EnumEntityConfiguration<TEnum>`
  - `QueryProjectionReferenceConfiguration`
- DB Interceptors
  - `AuditDetailsSaveChangesInterceptor` (not audit history, just create/update details for an entity)
  - `DomainEventSaveChangesInterceptor<TContext>`
- DB context abstract class (`ApplicationDbContext<TContext>`)
- Domain event dispatching infrastructure
- Repository pattern implementation (`Repository<TAggregateRoot>`)
- Query projection infrastruture
- Databse seeder interface `IDatabaseSeeder<TDbContext>`
- Service collection extension methods for configuring the above components

## Usage

1. Add a reference to `Lewee.Infrastructure.Data` package in your application project
2. Create a Entity Framwork DB context that inherits from `ApplicationDbContext<TContext>` ([example code](https://github.com/TheMagnificent11/lewee/blob/cab9ccd815cdba226c387921ec681560e5a2fec8/sample/Sample.Restaurant.Infrastructure/Data/RestaurantDbContext.cs#L9))
3. Add the following code (change according to your underlying database type)

```cs
// Without DB Seeder
builder.Services
  .AddDbContextFactory<MyDbContext>(options => options.UseSqlServer(connectionString))
  .AddLeweeDatabaseConfiguration<MyDbContext>(typeof(MyDomainEntity).Assembly);

// With DB Seeder
builder.Services
  .AddDbContextFactory<MyDbContext>(options => options.UseSqlServer(connectionString))
  .AddLeweeDatabaseConfigurationWithSeeder<MyDbContext, MyDbSeeder>(typeof(MyDomainEntity).Assembly);
```
