using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBillServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "PatientBillServices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientBillServices_BookingId",
                table: "PatientBillServices",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientBillServices_Bookings_BookingId",
                table: "PatientBillServices",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientBillServices_Bookings_BookingId",
                table: "PatientBillServices");

            migrationBuilder.DropIndex(
                name: "IX_PatientBillServices_BookingId",
                table: "PatientBillServices");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "PatientBillServices");
        }
    }
}
