using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria.Store.Data.Migrations;

public partial class AddOrderPizzaSnapshots : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.AddColumn<string>(
            name: "PizzaName",
            schema: "sto",
            table: "OrderPizza",
            type: "character varying(25)",
            maxLength: 25,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "PizzaPrice",
            schema: "sto",
            table: "OrderPizza",
            type: "numeric(5,2)",
            precision: 5,
            scale: 2,
            nullable: true);

        migrationBuilder.Sql(
            """
            UPDATE "sto"."OrderPizza" AS op
            SET "PizzaName" = p."Name", "PizzaPrice" = p."Price"
            FROM "sto"."Pizzas" AS p
            WHERE p."Id" = op."PizzaId";
            """);

        migrationBuilder.AlterColumn<string>(
            name: "PizzaName",
            schema: "sto",
            table: "OrderPizza",
            type: "character varying(25)",
            maxLength: 25,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(25)",
            oldMaxLength: 25,
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "PizzaPrice",
            schema: "sto",
            table: "OrderPizza",
            type: "numeric(5,2)",
            precision: 5,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(5,2)",
            oldPrecision: 5,
            oldScale: 2,
            oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropColumn(
            name: "PizzaName",
            schema: "sto",
            table: "OrderPizza");

        migrationBuilder.DropColumn(
            name: "PizzaPrice",
            schema: "sto",
            table: "OrderPizza");
    }
}
