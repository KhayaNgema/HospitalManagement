using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePackageModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Packages_DeliveryId",
                table: "Packages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CollectionDate",
                table: "Packages",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "PackageNumber",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "QrCodeImage",
                table: "Packages",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "Deliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_DeliveryId",
                table: "Packages",
                column: "DeliveryId",
                unique: true,
                filter: "[DeliveryId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Packages_DeliveryId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "PackageNumber",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "QrCodeImage",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Deliveries");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CollectionDate",
                table: "Packages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_DeliveryId",
                table: "Packages",
                column: "DeliveryId");
        }
    }
}
