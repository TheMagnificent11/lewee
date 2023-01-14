using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sample.Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MenuItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuItemTypes",
                schema: "res",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                schema: "res",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalSchema: "res",
                        principalTable: "MenuItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

#pragma warning disable SA1118 // Parameter should not span multiple lines
            migrationBuilder.InsertData(
                schema: "res",
                table: "MenuItemTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Food" },
                    { 1, "Drink" }
                });

            migrationBuilder.InsertData(
                schema: "res",
                table: "MenuItems",
                columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "IsDeleted", "ItemTypeId", "ModifiedAtUtc", "ModifiedBy", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("110d16d7-3ce5-49dd-a187-d3640fdb42b5"), new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7412), "System", false, 1, new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7412), "System", "Soft Drink", 3.50m },
                    { new Guid("1e1670cf-a80e-4421-a04f-6e28dc32a5d4"), new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7394), "System", false, 0, new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7394), "System", "Ice Cream", 5m },
                    { new Guid("58b91a73-682d-4696-b545-b493b56a0335"), new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7406), "System", false, 1, new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7406), "System", "Wine", 10m },
                    { new Guid("70da3fcf-381d-4285-88e6-794b4b57e5b5"), new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7384), "System", false, 0, new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7384), "System", "Pasta", 15m },
                    { new Guid("76495fb6-323e-4fbd-b0a4-5dbfcf61cef8"), new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7399), "System", false, 1, new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7400), "System", "Beer", 7.50m },
                    { new Guid("7fabe425-1d65-48d3-9ae4-caf5f27bbde8"), new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7343), "System", false, 0, new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7347), "System", "Pizza", 15m },
                    { new Guid("89669f56-63fe-4966-9028-f22f8d5a72f5"), new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7390), "System", false, 0, new DateTime(2023, 1, 14, 9, 16, 37, 673, DateTimeKind.Utc).AddTicks(7391), "System", "Garlic Bread", 4.50m }
                });
#pragma warning restore SA1118 // Parameter should not span multiple lines

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_ItemTypeId",
                schema: "res",
                table: "MenuItems",
                column: "ItemTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItems",
                schema: "res");

            migrationBuilder.DropTable(
                name: "MenuItemTypes",
                schema: "res");
        }
    }
}
