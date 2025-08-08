using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQeCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarCode",
                table: "MedicationStocks");

            migrationBuilder.AddColumn<int>(
                name: "MedicationId",
                table: "MedicationStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "QrCodeImage",
                table: "MedicationStocks",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationStocks_MedicationId",
                table: "MedicationStocks",
                column: "MedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationStocks_Medications_MedicationId",
                table: "MedicationStocks",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationStocks_Medications_MedicationId",
                table: "MedicationStocks");

            migrationBuilder.DropIndex(
                name: "IX_MedicationStocks_MedicationId",
                table: "MedicationStocks");

            migrationBuilder.DropColumn(
                name: "MedicationId",
                table: "MedicationStocks");

            migrationBuilder.DropColumn(
                name: "QrCodeImage",
                table: "MedicationStocks");

            migrationBuilder.AddColumn<string>(
                name: "BarCode",
                table: "MedicationStocks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
