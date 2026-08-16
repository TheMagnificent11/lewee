using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria.Store.Data.Migrations;

/// <inheritdoc />
public partial class RemoveStoreUsers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Users",
            schema: "sto");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            schema: "sto",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                ModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Users_ExternalId",
            schema: "sto",
            table: "Users",
            column: "ExternalId",
            unique: true);
    }
}
