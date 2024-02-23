using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sample.Pizzeria.Infrastructure.Migrations;

/// <inheritdoc />
public partial class PizzaOrders : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "dbo");

        migrationBuilder.CreateTable(
            name: "DomainEventReferences",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DomainEventAssemblyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                DomainEventClassName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                DomainEventJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                Dispatched = table.Column<bool>(type: "bit", nullable: false),
                PersistedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DispatchedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DomainEventReferences", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OrderStatuses",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderStatuses", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "QueryProjectionReferences",
            schema: "dbo",
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

        migrationBuilder.CreateTable(
            name: "Orders",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrderPlacedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeliveryAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                StatusId = table.Column<int>(type: "int", nullable: false),
                PreparedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                PickedUpForDeliveryDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeliveredDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Orders", x => x.Id);
                table.ForeignKey(
                    name: "FK_Orders_OrderStatuses_StatusId",
                    column: x => x.StatusId,
                    principalSchema: "dbo",
                    principalTable: "OrderStatuses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "OrderPizzas",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PizzaId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderPizzas", x => x.Id);
                table.ForeignKey(
                    name: "FK_OrderPizzas_Orders_OrderId",
                    column: x => x.OrderId,
                    principalSchema: "dbo",
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

#pragma warning disable SA1118 // Parameter should not span multiple lines
        migrationBuilder.InsertData(
            schema: "dbo",
            table: "OrderStatuses",
            columns: new[] { "Id", "Name" },
            values: new object[,]
            {
                { 0, "New" },
                { 1, "Submitted" },
                { 2, "Prepared" },
                { 3, "PickedUpForDelivery" },
                { 4, "Delivered" }
            });
#pragma warning restore SA1118 // Parameter should not span multiple lines

        migrationBuilder.CreateIndex(
            name: "IX_DomainEventReferences_Dispatched_PersistedAt",
            schema: "dbo",
            table: "DomainEventReferences",
            columns: new[] { "Dispatched", "PersistedAt" });

        migrationBuilder.CreateIndex(
            name: "IX_OrderPizzas_OrderId",
            schema: "dbo",
            table: "OrderPizzas",
            column: "OrderId");

        migrationBuilder.CreateIndex(
            name: "IX_Orders_StatusId",
            schema: "dbo",
            table: "Orders",
            column: "StatusId");

        migrationBuilder.CreateIndex(
            name: "IX_QueryProjectionReferences_QueryProjectionAssemblyName_QueryProjectionClassName_Key",
            schema: "dbo",
            table: "QueryProjectionReferences",
            columns: new[] { "QueryProjectionAssemblyName", "QueryProjectionClassName", "Key" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DomainEventReferences",
            schema: "dbo");

        migrationBuilder.DropTable(
            name: "OrderPizzas",
            schema: "dbo");

        migrationBuilder.DropTable(
            name: "QueryProjectionReferences",
            schema: "dbo");

        migrationBuilder.DropTable(
            name: "Orders",
            schema: "dbo");

        migrationBuilder.DropTable(
            name: "OrderStatuses",
            schema: "dbo");
    }
}
