using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sample.Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Orders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                schema: "res",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "res",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrderStatuses_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalSchema: "res",
                        principalTable: "OrderStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Tables_TableId",
                        column: x => x.TableId,
                        principalSchema: "res",
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

#pragma warning disable SA1118 // Parameter should not span multiple lines
            migrationBuilder.InsertData(
                schema: "res",
                table: "OrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Ordering" },
                    { 1, "Order Placed" },
                    { 2, "Updated" },
                    { 3, "Paid" }
                });
#pragma warning restore SA1118 // Parameter should not span multiple lines

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("2a150b61-8f9a-497f-a77a-2c701158b5a5"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5741), new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5742) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("3cd0f023-94e8-4114-b139-21d4955e1bab"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5746), new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5746) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("45694a13-30c1-4ff9-b7d2-8079657a6e29"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5735), new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5735) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("735e6fee-be38-4b02-a1e9-659e727c072e"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5732), new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5733) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("7c181a4b-43a9-4d3c-acdc-dc00a1f8a423"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5727), new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5727) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("830f9c25-7cbd-44b7-84b1-bd55973deca9"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5744), new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5744) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("87cbce7d-0daa-4ed5-9473-a1a473ca0cb5"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5739), new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5739) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("a6ce6962-20ec-4def-be6d-d568a12a022c"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5749), new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5749) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("dab4cd90-e6ca-48ec-b7a0-fcbe4e6e5805"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5702), new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5705) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("ec0fad21-c060-4315-a39f-6947deccd8fa"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5730), new DateTime(2023, 1, 11, 19, 57, 41, 169, DateTimeKind.Utc).AddTicks(5730) });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatusId",
                schema: "res",
                table: "Orders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableId",
                schema: "res",
                table: "Orders",
                column: "TableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders",
                schema: "res");

            migrationBuilder.DropTable(
                name: "OrderStatuses",
                schema: "res");

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("2a150b61-8f9a-497f-a77a-2c701158b5a5"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7869), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7870) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("3cd0f023-94e8-4114-b139-21d4955e1bab"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7874), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7874) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("45694a13-30c1-4ff9-b7d2-8079657a6e29"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7863), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7864) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("735e6fee-be38-4b02-a1e9-659e727c072e"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7861), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7861) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("7c181a4b-43a9-4d3c-acdc-dc00a1f8a423"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7856), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7856) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("830f9c25-7cbd-44b7-84b1-bd55973deca9"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7872), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7872) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("87cbce7d-0daa-4ed5-9473-a1a473ca0cb5"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7867), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7868) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("a6ce6962-20ec-4def-be6d-d568a12a022c"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7877), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7877) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("dab4cd90-e6ca-48ec-b7a0-fcbe4e6e5805"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7832), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7835) });

            migrationBuilder.UpdateData(
                schema: "res",
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("ec0fad21-c060-4315-a39f-6947deccd8fa"),
                columns: new[] { "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[] { new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7859), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7859) });
        }
    }
}
