using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteToimeRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "MedicalHistorys");

            migrationBuilder.RenameColumn(
                name: "Surgeries",
                table: "MedicalHistorys",
                newName: "Vitals");

            migrationBuilder.RenameColumn(
                name: "PreviousIllnesses",
                table: "MedicalHistorys",
                newName: "Treatment");

            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "MedicalHistorys",
                newName: "VisitDate");

            migrationBuilder.RenameColumn(
                name: "CurrentMedications",
                table: "MedicalHistorys",
                newName: "Symptoms");

            migrationBuilder.RenameColumn(
                name: "Allergies",
                table: "MedicalHistorys",
                newName: "Medications");

            migrationBuilder.AddColumn<string>(
                name: "ChiefComplaint",
                table: "MedicalHistorys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "MedicalHistorys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FollowUpInstructions",
                table: "MedicalHistorys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabResults",
                table: "MedicalHistorys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecordedAt",
                table: "MedicalHistorys",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiefComplaint",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "FollowUpInstructions",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "LabResults",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "RecordedAt",
                table: "MedicalHistorys");

            migrationBuilder.RenameColumn(
                name: "Vitals",
                table: "MedicalHistorys",
                newName: "Surgeries");

            migrationBuilder.RenameColumn(
                name: "VisitDate",
                table: "MedicalHistorys",
                newName: "LastUpdated");

            migrationBuilder.RenameColumn(
                name: "Treatment",
                table: "MedicalHistorys",
                newName: "PreviousIllnesses");

            migrationBuilder.RenameColumn(
                name: "Symptoms",
                table: "MedicalHistorys",
                newName: "CurrentMedications");

            migrationBuilder.RenameColumn(
                name: "Medications",
                table: "MedicalHistorys",
                newName: "Allergies");

            migrationBuilder.AddColumn<int>(
                name: "BloodGroup",
                table: "MedicalHistorys",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
