using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sample.Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviorToRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_MenuItemTypes_ItemTypeId",
                schema: "res",
                table: "MenuItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                schema: "res",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_MenuItemTypes_ItemTypeId",
                schema: "res",
                table: "MenuItems",
                column: "ItemTypeId",
                principalSchema: "res",
                principalTable: "MenuItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                schema: "res",
                table: "Orders",
                column: "OrderStatusId",
                principalSchema: "res",
                principalTable: "OrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_MenuItemTypes_ItemTypeId",
                schema: "res",
                table: "MenuItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                schema: "res",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_MenuItemTypes_ItemTypeId",
                schema: "res",
                table: "MenuItems",
                column: "ItemTypeId",
                principalSchema: "res",
                principalTable: "MenuItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                schema: "res",
                table: "Orders",
                column: "OrderStatusId",
                principalSchema: "res",
                principalTable: "OrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
