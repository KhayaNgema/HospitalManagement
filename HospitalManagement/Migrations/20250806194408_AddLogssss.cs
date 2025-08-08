using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddLogssss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicationUsageLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationId = table.Column<int>(type: "int", nullable: false),
                    DispensedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityDispensed = table.Column<int>(type: "int", nullable: false),
                    MedicationPescriptionId = table.Column<int>(type: "int", nullable: true),
                    DispensedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationUsageLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_MedicationUsageLogs_AspNetUsers_DispensedById",
                        column: x => x.DispensedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicationUsageLogs_MedicationPescription_MedicationPescriptionId",
                        column: x => x.MedicationPescriptionId,
                        principalTable: "MedicationPescription",
                        principalColumn: "MedicationPescriptionId");
                    table.ForeignKey(
                        name: "FK_MedicationUsageLogs_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicationUsageLogs_DispensedById",
                table: "MedicationUsageLogs",
                column: "DispensedById");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationUsageLogs_MedicationId",
                table: "MedicationUsageLogs",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationUsageLogs_MedicationPescriptionId",
                table: "MedicationUsageLogs",
                column: "MedicationPescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicationUsageLogs");
        }
    }
}
