using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_AspNetUsers_CreatedById",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Category_AspNetUsers_UpdatedById",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Category_CategoryId",
                table: "MenuItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_Category_UpdatedById",
                table: "Categories",
                newName: "IX_Categories_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Category_CreatedById",
                table: "Categories",
                newName: "IX_Categories_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_CreatedById",
                table: "Categories",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_UpdatedById",
                table: "Categories",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Categories_CategoryId",
                table: "MenuItems",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_CreatedById",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_UpdatedById",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Categories_CategoryId",
                table: "MenuItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_UpdatedById",
                table: "Category",
                newName: "IX_Category_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_CreatedById",
                table: "Category",
                newName: "IX_Category_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "CategoryId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Category_CategoryId",
                table: "MenuItems",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
