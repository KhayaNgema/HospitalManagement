using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMedHist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ward",
                table: "Admissions");

            migrationBuilder.AddColumn<int>(
                name: "CollectAfterCount",
                table: "MedicalHistorys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CollectionInterval",
                table: "MedicalHistorys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrescriptionType",
                table: "MedicalHistorys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UntilDate",
                table: "MedicalHistorys",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectAfterCount",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "CollectionInterval",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "PrescriptionType",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "UntilDate",
                table: "MedicalHistorys");

            migrationBuilder.AddColumn<int>(
                name: "Ward",
                table: "Admissions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
