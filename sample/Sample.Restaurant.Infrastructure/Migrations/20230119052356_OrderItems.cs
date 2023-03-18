using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sample.Restaurant.Infrastructure.Migrations;

/// <inheritdoc />
public partial class OrderItems : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "OrderItems",
            schema: "res",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MenuItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                Price = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_OrderItems_MenuItems_MenuItemId",
                    column: x => x.MenuItemId,
                    principalSchema: "res",
                    principalTable: "MenuItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_OrderItems_Orders_OrderId",
                    column: x => x.OrderId,
                    principalSchema: "res",
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_OrderItems_MenuItemId",
            schema: "res",
            table: "OrderItems",
            column: "MenuItemId");

        migrationBuilder.CreateIndex(
            name: "IX_OrderItems_OrderId",
            schema: "res",
            table: "OrderItems",
            column: "OrderId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OrderItems",
            schema: "res");
    }
}
