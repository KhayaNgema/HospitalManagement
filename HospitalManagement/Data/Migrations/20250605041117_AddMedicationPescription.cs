using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicationPescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Admissions_AdmissionId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "CollectAfterCount",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "CollectionInterval",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "PrescriptionType",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "UntilDate",
                table: "Admissions");

            migrationBuilder.RenameColumn(
                name: "AdmissionId",
                table: "Medications",
                newName: "MedicationPescriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Medications_AdmissionId",
                table: "Medications",
                newName: "IX_Medications_MedicationPescriptionId");

            migrationBuilder.AddColumn<int>(
                name: "MedicalHistoryId1",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedicationPescription",
                columns: table => new
                {
                    MedicationPescriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmissionId = table.Column<int>(type: "int", nullable: true),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UntilDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CollectAfterCount = table.Column<int>(type: "int", nullable: true),
                    CollectionInterval = table.Column<int>(type: "int", nullable: true),
                    PrescriptionType = table.Column<int>(type: "int", nullable: true),
                    HasDoneCollecting = table.Column<bool>(type: "bit", nullable: true),
                    NextCollectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationPescription", x => x.MedicationPescriptionId);
                    table.ForeignKey(
                        name: "FK_MedicationPescription_Admissions_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admissions",
                        principalColumn: "AdmissionId");
                    table.ForeignKey(
                        name: "FK_MedicationPescription_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_MedicationPescription_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medications_MedicalHistoryId1",
                table: "Medications",
                column: "MedicalHistoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationPescription_AdmissionId",
                table: "MedicationPescription",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationPescription_CreatedById",
                table: "MedicationPescription",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationPescription_UpdatedById",
                table: "MedicationPescription",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_MedicalHistorys_MedicalHistoryId1",
                table: "Medications",
                column: "MedicalHistoryId1",
                principalTable: "MedicalHistorys",
                principalColumn: "MedicalHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_MedicationPescription_MedicationPescriptionId",
                table: "Medications",
                column: "MedicationPescriptionId",
                principalTable: "MedicationPescription",
                principalColumn: "MedicationPescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_MedicalHistorys_MedicalHistoryId1",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_MedicationPescription_MedicationPescriptionId",
                table: "Medications");

            migrationBuilder.DropTable(
                name: "MedicationPescription");

            migrationBuilder.DropIndex(
                name: "IX_Medications_MedicalHistoryId1",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "MedicalHistoryId1",
                table: "Medications");

            migrationBuilder.RenameColumn(
                name: "MedicationPescriptionId",
                table: "Medications",
                newName: "AdmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_Medications_MedicationPescriptionId",
                table: "Medications",
                newName: "IX_Medications_AdmissionId");

            migrationBuilder.AddColumn<int>(
                name: "CollectAfterCount",
                table: "Admissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CollectionInterval",
                table: "Admissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrescriptionType",
                table: "Admissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UntilDate",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Admissions_AdmissionId",
                table: "Medications",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "AdmissionId");
        }
    }
}
