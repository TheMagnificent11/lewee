using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sample.Customers.Infrastructure.Migrations
{
    public partial class CustomersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DomainEventReferences",
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

            migrationBuilder.CreateIndex(
                name: "IX_DomainEventReferences_Dispatched_PersistedAt",
                table: "DomainEventReferences",
                columns: new[] { "Dispatched", "PersistedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "DomainEventReferences");
        }
    }
}
