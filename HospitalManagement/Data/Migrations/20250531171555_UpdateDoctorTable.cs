using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoctorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailabilityStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AvailableTimings_From",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AvailableTimings_To",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailabilityStatus",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AvailableTimings_From",
                table: "AspNetUsers",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AvailableTimings_To",
                table: "AspNetUsers",
                type: "time",
                nullable: true);
        }
    }
}
