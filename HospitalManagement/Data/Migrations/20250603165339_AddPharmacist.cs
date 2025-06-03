using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Doctor_Biography",
                table: "AspNetUsers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Doctor_Department",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Doctor_Education",
                table: "AspNetUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Doctor_LicenseNumber",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Doctor_YearsOfExperience",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Doctor_Biography",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_Department",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_Education",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_LicenseNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_YearsOfExperience",
                table: "AspNetUsers");
        }
    }
}
