using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddX_RayAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BodyParts",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Bookings",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DoctorId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScannerImage",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DoctorId",
                table: "Bookings",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_DoctorId",
                table: "Bookings",
                column: "DoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_DoctorId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_DoctorId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BodyParts",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ScannerImage",
                table: "Bookings");
        }
    }
}
