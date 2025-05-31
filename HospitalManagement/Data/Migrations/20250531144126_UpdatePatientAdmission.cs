using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePatientAdmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_AspNetUsers_UserId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "AttendingPhysicianId",
                table: "Admissions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Admissions",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Admissions_UserId",
                table: "Admissions",
                newName: "IX_Admissions_PatientId");

            migrationBuilder.AlterColumn<int>(
                name: "Department",
                table: "Admissions",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_AspNetUsers_PatientId",
                table: "Admissions",
                column: "PatientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_AspNetUsers_PatientId",
                table: "Admissions");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Admissions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Admissions_PatientId",
                table: "Admissions",
                newName: "IX_Admissions_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "Admissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "AttendingPhysicianId",
                table: "Admissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_AspNetUsers_UserId",
                table: "Admissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
