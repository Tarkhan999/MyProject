#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace FiorelloBackendPractice.Data;

/// <inheritdoc />
public partial class addProductsTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Categories",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>("nvarchar(max)", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Categories", x => x.Id); });

        migrationBuilder.CreateTable(
            "Products",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>("nvarchar(max)", nullable: false),
                Description = table.Column<string>("nvarchar(max)", nullable: false),
                Price = table.Column<double>("float", nullable: false),
                CategoryId = table.Column<int>("int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
                table.ForeignKey(
                    "FK_Products_Categories_CategoryId",
                    x => x.CategoryId,
                    "Categories",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "ProductImages",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Image = table.Column<string>("nvarchar(max)", nullable: false),
                IsMain = table.Column<bool>("bit", nullable: false),
                ProductId = table.Column<int>("int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductImages", x => x.Id);
                table.ForeignKey(
                    "FK_ProductImages_Products_ProductId",
                    x => x.ProductId,
                    "Products",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_ProductImages_ProductId",
            "ProductImages",
            "ProductId");

        migrationBuilder.CreateIndex(
            "IX_Products_CategoryId",
            "Products",
            "CategoryId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "ProductImages");

        migrationBuilder.DropTable(
            "Products");

        migrationBuilder.DropTable(
            "Categories");
    }
}