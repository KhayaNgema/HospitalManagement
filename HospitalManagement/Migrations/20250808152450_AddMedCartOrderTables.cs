using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    public partial class AddMedCartOrderTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // MedicationCarts table
            migrationBuilder.CreateTable(
                name: "MedicationCarts",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationCarts", x => x.CartId);
                });

            // MedicationOrders table
            migrationBuilder.CreateTable(
                name: "MedicationOrders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PharmacistId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationOrders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_MedicationOrders_AspNetUsers_PharmacistId",
                        column: x => x.PharmacistId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            // MedicationCartItems table
            migrationBuilder.CreateTable(
                name: "MedicationCartItems",
                columns: table => new
                {
                    CartItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PharmacistId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationCartItems", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_MedicationCartItems_AspNetUsers_PharmacistId",
                        column: x => x.PharmacistId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_MedicationCartItems_MedicationCarts_CartId",
                        column: x => x.CartId,
                        principalTable: "MedicationCarts",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_MedicationCartItems_MedicationStocks_StockId",
                        column: x => x.StockId,
                        principalTable: "MedicationStocks",
                        principalColumn: "StockId",
                        onDelete: ReferentialAction.NoAction);
                });

            // MedicationOrderItems table
            migrationBuilder.CreateTable(
                name: "MedicationOrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationOrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_MedicationOrderItems_MedicationOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "MedicationOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_MedicationOrderItems_MedicationStocks_StockId",
                        column: x => x.StockId,
                        principalTable: "MedicationStocks",
                        principalColumn: "StockId",
                        onDelete: ReferentialAction.NoAction);
                });

            // Indexes for MedicationCartItems
            migrationBuilder.CreateIndex(
                name: "IX_MedicationCartItems_PharmacistId",
                table: "MedicationCartItems",
                column: "PharmacistId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationCartItems_CartId",
                table: "MedicationCartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationCartItems_StockId",
                table: "MedicationCartItems",
                column: "StockId");

            // Indexes for MedicationOrderItems
            migrationBuilder.CreateIndex(
                name: "IX_MedicationOrderItems_OrderId",
                table: "MedicationOrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationOrderItems_StockId",
                table: "MedicationOrderItems",
                column: "StockId");

            // Index for MedicationOrders
            migrationBuilder.CreateIndex(
                name: "IX_MedicationOrders_PharmacistId",
                table: "MedicationOrders",
                column: "PharmacistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MedicationCartItems");
            migrationBuilder.DropTable(name: "MedicationOrderItems");
            migrationBuilder.DropTable(name: "MedicationCarts");
            migrationBuilder.DropTable(name: "MedicationOrders");
        }
    }
}
