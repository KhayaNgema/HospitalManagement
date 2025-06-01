using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMedical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_MedicalHistorys_MedicalHistoryId",
                table: "Admissions");

            migrationBuilder.RenameColumn(
                name: "MedicalHistoryId",
                table: "Admissions",
                newName: "PatientMedicalHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Admissions_MedicalHistoryId",
                table: "Admissions",
                newName: "IX_Admissions_PatientMedicalHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_PatientMedicalHistories_PatientMedicalHistoryId",
                table: "Admissions",
                column: "PatientMedicalHistoryId",
                principalTable: "PatientMedicalHistories",
                principalColumn: "PatientMedicalHistoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_PatientMedicalHistories_PatientMedicalHistoryId",
                table: "Admissions");

            migrationBuilder.RenameColumn(
                name: "PatientMedicalHistoryId",
                table: "Admissions",
                newName: "MedicalHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Admissions_PatientMedicalHistoryId",
                table: "Admissions",
                newName: "IX_Admissions_MedicalHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_MedicalHistorys_MedicalHistoryId",
                table: "Admissions",
                column: "MedicalHistoryId",
                principalTable: "MedicalHistorys",
                principalColumn: "MedicalHistoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
