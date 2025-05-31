using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAuditCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Category",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Category",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Category",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Category",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CreatedById",
                table: "Category",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Category_UpdatedById",
                table: "Category",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_AspNetUsers_CreatedById",
                table: "Category",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Category_AspNetUsers_UpdatedById",
                table: "Category",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_AspNetUsers_CreatedById",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Category_AspNetUsers_UpdatedById",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_CreatedById",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_UpdatedById",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Category");
        }
    }
}
