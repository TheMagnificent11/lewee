using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lewee.Auth.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddRolesAndSiteAdministrator : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.AddColumn<Guid[]>(
            name: "RoleIds",
            schema: "auth",
            table: "UserTenantMemberships",
            type: "uuid[]",
            nullable: false,
            defaultValue: Array.Empty<Guid>());

        migrationBuilder.AddColumn<bool>(
            name: "IsSiteAdministrator",
            schema: "auth",
            table: "Users",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "Roles",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                ModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Roles", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Roles_Code",
            schema: "auth",
            table: "Roles",
            column: "Code",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropTable(
            name: "Roles",
            schema: "auth");

        migrationBuilder.DropColumn(
            name: "RoleIds",
            schema: "auth",
            table: "UserTenantMemberships");

        migrationBuilder.DropColumn(
            name: "IsSiteAdministrator",
            schema: "auth",
            table: "Users");
    }
}
