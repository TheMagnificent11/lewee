using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sample.Restaurant.Infrastructure.Migrations;

/// <inheritdoc />
public partial class DomainEventReferencesUserIdColumns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "ClientId",
            schema: "res",
            table: "DomainEventReferences",
            newName: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "UserId",
            schema: "res",
            table: "DomainEventReferences",
            newName: "ClientId");
    }
}
