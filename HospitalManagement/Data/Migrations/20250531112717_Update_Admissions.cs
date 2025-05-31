using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Admissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "BloodType",
                table: "AspNetUsers",
                type: "int",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlternatePhoneNumber",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AlternatePhoneNumber",
                table: "Admissions");

            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "AspNetUsers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
