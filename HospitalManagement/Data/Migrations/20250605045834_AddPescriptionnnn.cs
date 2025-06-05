using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPescriptionnnn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UntilDate",
                table: "MedicationPescription",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextCollectionDate",
                table: "MedicationPescription",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "MedicationPescription",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCollectionDate",
                table: "MedicationPescription",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationPescription_BookingId",
                table: "MedicationPescription",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationPescription_Admissions_BookingId",
                table: "MedicationPescription",
                column: "BookingId",
                principalTable: "Admissions",
                principalColumn: "AdmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationPescription_Admissions_BookingId",
                table: "MedicationPescription");

            migrationBuilder.DropIndex(
                name: "IX_MedicationPescription_BookingId",
                table: "MedicationPescription");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "MedicationPescription");

            migrationBuilder.DropColumn(
                name: "LastCollectionDate",
                table: "MedicationPescription");

            migrationBuilder.AlterColumn<string>(
                name: "UntilDate",
                table: "MedicationPescription",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextCollectionDate",
                table: "MedicationPescription",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
