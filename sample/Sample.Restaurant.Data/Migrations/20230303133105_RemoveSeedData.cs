using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sample.Restaurant.Infrastructure.Migrations;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
public partial class RemoveSeedData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            schema: "res",
            table: "MenuItems",
            keyColumn: "Id",
            keyValue: new Guid("110d16d7-3ce5-49dd-a187-d3640fdb42b5"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "MenuItems",
            keyColumn: "Id",
            keyValue: new Guid("1e1670cf-a80e-4421-a04f-6e28dc32a5d4"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "MenuItems",
            keyColumn: "Id",
            keyValue: new Guid("58b91a73-682d-4696-b545-b493b56a0335"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "MenuItems",
            keyColumn: "Id",
            keyValue: new Guid("70da3fcf-381d-4285-88e6-794b4b57e5b5"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "MenuItems",
            keyColumn: "Id",
            keyValue: new Guid("76495fb6-323e-4fbd-b0a4-5dbfcf61cef8"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "MenuItems",
            keyColumn: "Id",
            keyValue: new Guid("7fabe425-1d65-48d3-9ae4-caf5f27bbde8"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "MenuItems",
            keyColumn: "Id",
            keyValue: new Guid("89669f56-63fe-4966-9028-f22f8d5a72f5"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "Tables",
            keyColumn: "Id",
            keyValue: new Guid("2a150b61-8f9a-497f-a77a-2c701158b5a5"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "Tables",
            keyColumn: "Id",
            keyValue: new Guid("3cd0f023-94e8-4114-b139-21d4955e1bab"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "Tables",
            keyColumn: "Id",
            keyValue: new Guid("45694a13-30c1-4ff9-b7d2-8079657a6e29"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "Tables",
            keyColumn: "Id",
            keyValue: new Guid("735e6fee-be38-4b02-a1e9-659e727c072e"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "Tables",
            keyColumn: "Id",
            keyValue: new Guid("7c181a4b-43a9-4d3c-acdc-dc00a1f8a423"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "Tables",
            keyColumn: "Id",
            keyValue: new Guid("830f9c25-7cbd-44b7-84b1-bd55973deca9"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "Tables",
            keyColumn: "Id",
            keyValue: new Guid("87cbce7d-0daa-4ed5-9473-a1a473ca0cb5"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "Tables",
            keyColumn: "Id",
            keyValue: new Guid("a6ce6962-20ec-4def-be6d-d568a12a022c"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "Tables",
            keyColumn: "Id",
            keyValue: new Guid("dab4cd90-e6ca-48ec-b7a0-fcbe4e6e5805"));

        migrationBuilder.DeleteData(
            schema: "res",
            table: "Tables",
            keyColumn: "Id",
            keyValue: new Guid("ec0fad21-c060-4315-a39f-6947deccd8fa"));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
#pragma warning disable SA1118 // Parameter should not span multiple lines
        migrationBuilder.InsertData(
            schema: "res",
            table: "MenuItems",
            columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "IsDeleted", "ItemTypeId", "ModifiedAtUtc", "ModifiedBy", "Name", "Price" },
            values: new object[,]
            {
                { new Guid("110d16d7-3ce5-49dd-a187-d3640fdb42b5"), new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3792), "System", false, 1, new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3792), "System", "Soft Drink", 3.50m },
                { new Guid("1e1670cf-a80e-4421-a04f-6e28dc32a5d4"), new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3778), "System", false, 0, new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3779), "System", "Ice Cream", 5m },
                { new Guid("58b91a73-682d-4696-b545-b493b56a0335"), new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3788), "System", false, 1, new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3788), "System", "Wine", 10m },
                { new Guid("70da3fcf-381d-4285-88e6-794b4b57e5b5"), new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3768), "System", false, 0, new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3769), "System", "Pasta", 15m },
                { new Guid("76495fb6-323e-4fbd-b0a4-5dbfcf61cef8"), new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3783), "System", false, 1, new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3783), "System", "Beer", 7.50m },
                { new Guid("7fabe425-1d65-48d3-9ae4-caf5f27bbde8"), new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3751), "System", false, 0, new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3753), "System", "Pizza", 15m },
                { new Guid("89669f56-63fe-4966-9028-f22f8d5a72f5"), new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3774), "System", false, 0, new DateTime(2023, 2, 25, 2, 41, 17, 710, DateTimeKind.Utc).AddTicks(3775), "System", "Garlic Bread", 4.50m }
            });

        migrationBuilder.InsertData(
            schema: "res",
            table: "Tables",
            columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "IsDeleted", "IsInUse", "ModifiedAtUtc", "ModifiedBy", "TableNumber" },
            values: new object[,]
            {
                { new Guid("2a150b61-8f9a-497f-a77a-2c701158b5a5"), new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4470), "System", false, false, new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4471), "System", 7 },
                { new Guid("3cd0f023-94e8-4114-b139-21d4955e1bab"), new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4476), "System", false, false, new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4476), "System", 9 },
                { new Guid("45694a13-30c1-4ff9-b7d2-8079657a6e29"), new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4411), "System", false, false, new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4412), "System", 5 },
                { new Guid("735e6fee-be38-4b02-a1e9-659e727c072e"), new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4409), "System", false, false, new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4409), "System", 4 },
                { new Guid("7c181a4b-43a9-4d3c-acdc-dc00a1f8a423"), new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4402), "System", false, false, new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4402), "System", 2 },
                { new Guid("830f9c25-7cbd-44b7-84b1-bd55973deca9"), new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4473), "System", false, false, new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4473), "System", 8 },
                { new Guid("87cbce7d-0daa-4ed5-9473-a1a473ca0cb5"), new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4466), "System", false, false, new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4467), "System", 6 },
                { new Guid("a6ce6962-20ec-4def-be6d-d568a12a022c"), new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4479), "System", false, false, new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4480), "System", 10 },
                { new Guid("dab4cd90-e6ca-48ec-b7a0-fcbe4e6e5805"), new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4369), "System", false, false, new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4373), "System", 1 },
                { new Guid("ec0fad21-c060-4315-a39f-6947deccd8fa"), new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4405), "System", false, false, new DateTime(2023, 2, 25, 2, 41, 17, 709, DateTimeKind.Utc).AddTicks(4406), "System", 3 }
            });
#pragma warning restore SA1118 // Parameter should not span multiple lines
    }
}
