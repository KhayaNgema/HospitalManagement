using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientMedicalHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Medications",
                table: "MedicalHistorys");

            migrationBuilder.AddColumn<int>(
                name: "MedicalHistoryId",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MedicalHistorys",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "MedicalHistorys",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "MedicalHistorys",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PatientMedicalHistoryId",
                table: "MedicalHistorys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "MedicalHistorys",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PatientMedicalHistories",
                columns: table => new
                {
                    PatientMedicalHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QrCodeImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedicalHistories", x => x.PatientMedicalHistoryId);
                    table.ForeignKey(
                        name: "FK_PatientMedicalHistories_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medications_MedicalHistoryId",
                table: "Medications",
                column: "MedicalHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistorys_CreatedById",
                table: "MedicalHistorys",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistorys_PatientMedicalHistoryId",
                table: "MedicalHistorys",
                column: "PatientMedicalHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistorys_UpdatedById",
                table: "MedicalHistorys",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedicalHistories_PatientId",
                table: "PatientMedicalHistories",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalHistorys_AspNetUsers_CreatedById",
                table: "MedicalHistorys",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalHistorys_AspNetUsers_UpdatedById",
                table: "MedicalHistorys",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalHistorys_PatientMedicalHistories_PatientMedicalHistoryId",
                table: "MedicalHistorys",
                column: "PatientMedicalHistoryId",
                principalTable: "PatientMedicalHistories",
                principalColumn: "PatientMedicalHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_MedicalHistorys_MedicalHistoryId",
                table: "Medications",
                column: "MedicalHistoryId",
                principalTable: "MedicalHistorys",
                principalColumn: "MedicalHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalHistorys_AspNetUsers_CreatedById",
                table: "MedicalHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalHistorys_AspNetUsers_UpdatedById",
                table: "MedicalHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalHistorys_PatientMedicalHistories_PatientMedicalHistoryId",
                table: "MedicalHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_MedicalHistorys_MedicalHistoryId",
                table: "Medications");

            migrationBuilder.DropTable(
                name: "PatientMedicalHistories");

            migrationBuilder.DropIndex(
                name: "IX_Medications_MedicalHistoryId",
                table: "Medications");

            migrationBuilder.DropIndex(
                name: "IX_MedicalHistorys_CreatedById",
                table: "MedicalHistorys");

            migrationBuilder.DropIndex(
                name: "IX_MedicalHistorys_PatientMedicalHistoryId",
                table: "MedicalHistorys");

            migrationBuilder.DropIndex(
                name: "IX_MedicalHistorys_UpdatedById",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "MedicalHistoryId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "PatientMedicalHistoryId",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "MedicalHistorys");

            migrationBuilder.AddColumn<string>(
                name: "Medications",
                table: "MedicalHistorys",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
