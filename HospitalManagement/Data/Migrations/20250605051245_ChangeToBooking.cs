using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationPescription_Admissions_BookingId",
                table: "MedicationPescription");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationPescription_Bookings_BookingId",
                table: "MedicationPescription",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationPescription_Bookings_BookingId",
                table: "MedicationPescription");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationPescription_Admissions_BookingId",
                table: "MedicationPescription",
                column: "BookingId",
                principalTable: "Admissions",
                principalColumn: "AdmissionId");
        }
    }
}
