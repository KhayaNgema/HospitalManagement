using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMedddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Medications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MedicationCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationCategories", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_MedicationCategories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_MedicationCategories_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medications_CategoryId",
                table: "Medications",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationCategories_CreatedById",
                table: "MedicationCategories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationCategories_UpdatedById",
                table: "MedicationCategories",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_MedicationCategories_CategoryId",
                table: "Medications",
                column: "CategoryId",
                principalTable: "MedicationCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_MedicationCategories_CategoryId",
                table: "Medications");

            migrationBuilder.DropTable(
                name: "MedicationCategories");

            migrationBuilder.DropIndex(
                name: "IX_Medications_CategoryId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Medications");
        }
    }
}
