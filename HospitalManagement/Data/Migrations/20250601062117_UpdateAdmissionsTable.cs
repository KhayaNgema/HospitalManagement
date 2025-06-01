using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdmissionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentDiagnosis",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "CurrentMedications",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "HeightCm",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "Immunizations",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "Surgeries",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                table: "Admissions");

            migrationBuilder.AddColumn<float>(
                name: "HeightCm",
                table: "MedicalHistorys",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Immunizations",
                table: "MedicalHistorys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surgeries",
                table: "MedicalHistorys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "WeightKg",
                table: "MedicalHistorys",
                type: "real",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Ward",
                table: "Admissions",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Admissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BookingId",
                table: "Admissions",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Bookings_BookingId",
                table: "Admissions",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Bookings_BookingId",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_BookingId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "HeightCm",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "Immunizations",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "Surgeries",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                table: "MedicalHistorys");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Admissions");

            migrationBuilder.AlterColumn<string>(
                name: "Ward",
                table: "Admissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "CurrentDiagnosis",
                table: "Admissions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentMedications",
                table: "Admissions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "HeightCm",
                table: "Admissions",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Immunizations",
                table: "Admissions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surgeries",
                table: "Admissions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "WeightKg",
                table: "Admissions",
                type: "real",
                nullable: true);
        }
    }
}
