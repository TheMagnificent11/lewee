using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sample.Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReadModelReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReadModelReferences",
                schema: "res",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReadModelAssemblyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReadModelClassName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReadModelJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadModelReferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReadModelReferences_ReadModelAssemblyName_ReadModelClassName_Key",
                schema: "res",
                table: "ReadModelReferences",
                columns: new[] { "ReadModelAssemblyName", "ReadModelClassName", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReadModelReferences",
                schema: "res");
        }
    }
}
