using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_MedicationPescription_MedicationPescriptionId",
                table: "Medications");

            migrationBuilder.DropIndex(
                name: "IX_Medications_MedicationPescriptionId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "MedicationPescriptionId",
                table: "Medications");

            migrationBuilder.CreateTable(
                name: "MedicationPescription_Medication",
                columns: table => new
                {
                    MedicationPescriptionId = table.Column<int>(type: "int", nullable: false),
                    MedicationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationPescription_Medication", x => new { x.MedicationPescriptionId, x.MedicationId });
                    table.ForeignKey(
                        name: "FK_MedicationPescription_Medication_MedicationPescription_MedicationPescriptionId",
                        column: x => x.MedicationPescriptionId,
                        principalTable: "MedicationPescription",
                        principalColumn: "MedicationPescriptionId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_MedicationPescription_Medication_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicationPescription_Medication_MedicationId",
                table: "MedicationPescription_Medication",
                column: "MedicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicationPescription_Medication");

            migrationBuilder.AddColumn<int>(
                name: "MedicationPescriptionId",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medications_MedicationPescriptionId",
                table: "Medications",
                column: "MedicationPescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_MedicationPescription_MedicationPescriptionId",
                table: "Medications",
                column: "MedicationPescriptionId",
                principalTable: "MedicationPescription",
                principalColumn: "MedicationPescriptionId");
        }
    }
}
