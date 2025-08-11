using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeliveryItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryPackageItems",
                columns: table => new
                {
                    DeliveryPackageItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryRequestId = table.Column<int>(type: "int", nullable: false),
                    MedicationId = table.Column<int>(type: "int", nullable: false),
                    IsPackaged = table.Column<bool>(type: "bit", nullable: false),
                    IsCollected = table.Column<bool>(type: "bit", nullable: false),
                    PackagedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CollectionAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPackageItems", x => x.DeliveryPackageItemId);
                    table.ForeignKey(
                        name: "FK_DeliveryPackageItems_DeliveryRequests_DeliveryRequestId",
                        column: x => x.DeliveryRequestId,
                        principalTable: "DeliveryRequests",
                        principalColumn: "DeliveryRequestId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DeliveryPackageItems_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPackageItems_DeliveryRequestId",
                table: "DeliveryPackageItems",
                column: "DeliveryRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPackageItems_MedicationId",
                table: "DeliveryPackageItems",
                column: "MedicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryPackageItems");
        }
    }
}
