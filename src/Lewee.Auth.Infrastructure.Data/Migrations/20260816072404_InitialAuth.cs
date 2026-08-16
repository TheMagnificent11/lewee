using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lewee.Auth.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class InitialAuth : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "auth");

        migrationBuilder.CreateTable(
            name: "DomainEventReferences",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                DomainEventAssemblyName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                DomainEventClassName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                DomainEventJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                Dispatched = table.Column<bool>(type: "boolean", nullable: false),
                PersistedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                DispatchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DomainEventReferences", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "QueryProjectionReferences",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                QueryProjectionAssemblyName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                QueryProjectionClassName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                QueryProjectionJson = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_QueryProjectionReferences", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Tenants",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                table.PrimaryKey("PK_Tenants", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            schema: "auth",
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
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserTenantMemberships",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "text", nullable: false),
                ModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserTenantMemberships", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserTenantMemberships_Tenants_TenantId",
                    column: x => x.TenantId,
                    principalSchema: "auth",
                    principalTable: "Tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserTenantMemberships_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: "auth",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_DomainEventReferences_Dispatched_PersistedAt",
            schema: "auth",
            table: "DomainEventReferences",
            columns: new[] { "Dispatched", "PersistedAt" });

        migrationBuilder.CreateIndex(
            name: "IX_QueryProjectionReferences_QueryProjectionAssemblyName_Query~",
            schema: "auth",
            table: "QueryProjectionReferences",
            columns: new[] { "QueryProjectionAssemblyName", "QueryProjectionClassName", "Key" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Users_ExternalId",
            schema: "auth",
            table: "Users",
            column: "ExternalId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserTenantMemberships_TenantId",
            schema: "auth",
            table: "UserTenantMemberships",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_UserTenantMemberships_UserId_TenantId",
            schema: "auth",
            table: "UserTenantMemberships",
            columns: new[] { "UserId", "TenantId" },
            unique: true);

        migrationBuilder.Sql(
            """
            DO $$
            BEGIN
                IF to_regclass('"sto"."Users"') IS NOT NULL THEN
                    INSERT INTO auth."Users"
                        ("Id", "ExternalId", "CreatedBy", "CreatedAtUtc",
                         "ModifiedBy", "ModifiedAtUtc", "IsDeleted")
                    SELECT
                        "Id", "ExternalId", "CreatedBy", "CreatedAtUtc",
                        "ModifiedBy", "ModifiedAtUtc", "IsDeleted"
                    FROM sto."Users"
                    ON CONFLICT ("ExternalId") DO NOTHING;
                END IF;
            END $$;
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DomainEventReferences",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "QueryProjectionReferences",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "UserTenantMemberships",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "Tenants",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "Users",
            schema: "auth");
    }
}
