using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDischargeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdmissionId",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "UntilDate",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medications_AdmissionId",
                table: "Medications",
                column: "AdmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Admissions_AdmissionId",
                table: "Medications",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "AdmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Admissions_AdmissionId",
                table: "Medications");

            migrationBuilder.DropIndex(
                name: "IX_Medications_AdmissionId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "AdmissionId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "CollectAfterCount",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "CollectionInterval",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "UntilDate",
                table: "Admissions");
        }
    }
}
