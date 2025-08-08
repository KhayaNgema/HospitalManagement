using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class STockDepMed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DosageForm",
                table: "MedicationStocks");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "MedicationStocks");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "MedicationStocks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DosageForm",
                table: "MedicationStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Strength",
                table: "MedicationStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnitOfMeasure",
                table: "MedicationStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
