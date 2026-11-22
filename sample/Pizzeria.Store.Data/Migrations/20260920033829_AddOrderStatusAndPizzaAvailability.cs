using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria.Store.Data.Migrations;

/// <inheritdoc />
public partial class AddOrderStatusAndPizzaAvailability : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.AddColumn<bool>(
            name: "IsAvailable",
            schema: "sto",
            table: "Pizzas",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "Status",
            schema: "sto",
            table: "Orders",
            type: "character varying(30)",
            maxLength: 30,
            nullable: false,
            defaultValue: string.Empty);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropColumn(
            name: "IsAvailable",
            schema: "sto",
            table: "Pizzas");

        migrationBuilder.DropColumn(
            name: "Status",
            schema: "sto",
            table: "Orders");
    }
}
