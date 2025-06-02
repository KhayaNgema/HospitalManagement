using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelXRayAndBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginalAppointmentBookingId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginalBookingId",
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
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Bookings_OriginalAppointmentBookingId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_OriginalAppointmentBookingId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "OriginalAppointmentBookingId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "OriginalBookingId",
                table: "Bookings");
        }
    }
}
