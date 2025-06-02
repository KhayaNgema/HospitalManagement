using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBookingOrigin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Bookings_OriginalAppointmentBookingId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_OriginalAppointmentBookingId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "OriginalAppointmentBookingId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_OriginalBookingId",
                table: "Bookings",
                column: "OriginalBookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Bookings_OriginalBookingId",
                table: "Bookings",
                column: "OriginalBookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Bookings_OriginalBookingId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_OriginalBookingId",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "OriginalAppointmentBookingId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_OriginalAppointmentBookingId",
                table: "Bookings",
                column: "OriginalAppointmentBookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Bookings_OriginalAppointmentBookingId",
                table: "Bookings",
                column: "OriginalAppointmentBookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
