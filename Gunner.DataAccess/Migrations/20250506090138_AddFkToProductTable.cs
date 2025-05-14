using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gunner.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddFkToProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "ProductID",
                keyValue: 1,
                column: "CategoryID",
                value: 3);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "ProductID",
                keyValue: 2,
                column: "CategoryID",
                value: 3);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "ProductID",
                keyValue: 3,
                column: "CategoryID",
                value: 3);

            migrationBuilder.CreateIndex(
                name: "IX_products_CategoryID",
                table: "products",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_CategoryID",
                table: "products",
                column: "CategoryID",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_CategoryID",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_CategoryID",
                table: "products");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "products");
        }
    }
}
