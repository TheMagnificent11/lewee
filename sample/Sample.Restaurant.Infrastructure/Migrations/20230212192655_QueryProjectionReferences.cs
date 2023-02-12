using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sample.Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class QueryProjectionReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QueryProjectionReferences",
                schema: "res",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QueryProjectionAssemblyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    QueryProjectionClassName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    QueryProjectionJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryProjectionReferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QueryProjectionReferences_QueryProjectionAssemblyName_QueryProjectionClassName_Key",
                schema: "res",
                table: "QueryProjectionReferences",
                columns: new[] { "QueryProjectionAssemblyName", "QueryProjectionClassName", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueryProjectionReferences",
                schema: "res");
        }
    }
}
