using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sample.Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "res");

            migrationBuilder.CreateTable(
                name: "DomainEventReferences",
                schema: "res",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DomainEventAssemblyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DomainEventClassName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DomainEventJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    Dispatched = table.Column<bool>(type: "bit", nullable: false),
                    PersistedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DispatchedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainEventReferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                schema: "res",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TableNumber = table.Column<int>(type: "int", nullable: false),
                    IsInUse = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                });

#pragma warning disable SA1118 // Parameter should not span multiple lines
            migrationBuilder.InsertData(
                schema: "res",
                table: "Tables",
                columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "IsDeleted", "IsInUse", "ModifiedAtUtc", "ModifiedBy", "TableNumber" },
                values: new object[,]
                {
                    { new Guid("2a150b61-8f9a-497f-a77a-2c701158b5a5"), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7869), "System", false, false, new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7870), "System", 7 },
                    { new Guid("3cd0f023-94e8-4114-b139-21d4955e1bab"), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7874), "System", false, false, new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7874), "System", 9 },
                    { new Guid("45694a13-30c1-4ff9-b7d2-8079657a6e29"), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7863), "System", false, false, new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7864), "System", 5 },
                    { new Guid("735e6fee-be38-4b02-a1e9-659e727c072e"), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7861), "System", false, false, new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7861), "System", 4 },
                    { new Guid("7c181a4b-43a9-4d3c-acdc-dc00a1f8a423"), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7856), "System", false, false, new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7856), "System", 2 },
                    { new Guid("830f9c25-7cbd-44b7-84b1-bd55973deca9"), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7872), "System", false, false, new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7872), "System", 8 },
                    { new Guid("87cbce7d-0daa-4ed5-9473-a1a473ca0cb5"), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7867), "System", false, false, new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7868), "System", 6 },
                    { new Guid("a6ce6962-20ec-4def-be6d-d568a12a022c"), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7877), "System", false, false, new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7877), "System", 10 },
                    { new Guid("dab4cd90-e6ca-48ec-b7a0-fcbe4e6e5805"), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7832), "System", false, false, new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7835), "System", 1 },
                    { new Guid("ec0fad21-c060-4315-a39f-6947deccd8fa"), new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7859), "System", false, false, new DateTime(2023, 1, 7, 19, 31, 47, 226, DateTimeKind.Utc).AddTicks(7859), "System", 3 }
                });
#pragma warning restore SA1118 // Parameter should not span multiple lines

            migrationBuilder.CreateIndex(
                name: "IX_DomainEventReferences_Dispatched_PersistedAt",
                schema: "res",
                table: "DomainEventReferences",
                columns: new[] { "Dispatched", "PersistedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Tables_TableNumber",
                schema: "res",
                table: "Tables",
                column: "TableNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEventReferences",
                schema: "res");

            migrationBuilder.DropTable(
                name: "Tables",
                schema: "res");
        }
    }
}
