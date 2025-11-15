using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria.Store.Data.Migrations;

/// <inheritdoc />
public partial class AddCustomer : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Customers",
            schema: "sto",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                ModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Customers_ExternalId",
            schema: "sto",
            table: "Customers",
            column: "ExternalId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Customers",
            schema: "sto");
    }
}
