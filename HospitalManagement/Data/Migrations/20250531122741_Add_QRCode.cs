using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_QRCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternatePhoneNumber",
                table: "Admissions");

            migrationBuilder.AddColumn<string>(
                name: "BookingReference",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "QrCodeImage",
                table: "Bookings",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<int>(
                name: "PatientStatus",
                table: "Admissions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingReference",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "QrCodeImage",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "PatientStatus",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AlternatePhoneNumber",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
