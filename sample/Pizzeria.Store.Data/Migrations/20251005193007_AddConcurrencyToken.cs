using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria.Store.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConcurrencyToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "sto",
                table: "Pizzas",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "sto",
                table: "Orders",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "sto",
                table: "Pizzas");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "sto",
                table: "Orders");
        }
    }
}
