using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Pizzeria.Store.Data;

#nullable disable

namespace Pizzeria.Store.Data.Migrations;

[DbContext(typeof(StoreDbContext))]
[Migration("20261030214300_AddOrderRevision")]
public partial class AddOrderRevision : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.AddColumn<long>(
            name: "Revision",
            schema: "sto",
            table: "Orders",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropColumn(
            name: "Revision",
            schema: "sto",
            table: "Orders");
    }
}
