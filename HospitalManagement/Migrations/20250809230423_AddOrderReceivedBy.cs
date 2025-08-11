using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderReceivedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceivedById",
                table: "MedicationOrders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationOrders_ReceivedById",
                table: "MedicationOrders",
                column: "ReceivedById");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationOrders_AspNetUsers_ReceivedById",
                table: "MedicationOrders",
                column: "ReceivedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationOrders_AspNetUsers_ReceivedById",
                table: "MedicationOrders");

            migrationBuilder.DropIndex(
                name: "IX_MedicationOrders_ReceivedById",
                table: "MedicationOrders");

            migrationBuilder.DropColumn(
                name: "ReceivedById",
                table: "MedicationOrders");
        }
    }
}
