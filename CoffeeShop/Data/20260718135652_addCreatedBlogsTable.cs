#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace FiorelloBackendPractice.Data;

/// <inheritdoc />
public partial class addCreatedBlogsTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Blogs",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>("nvarchar(max)", nullable: false),
                Description = table.Column<string>("nvarchar(max)", nullable: false),
                Image = table.Column<string>("nvarchar(max)", nullable: false),
                Link = table.Column<string>("nvarchar(max)", nullable: false),
                DateCreated = table.Column<DateTime>("datetime2", nullable: false),
                ViewsCount = table.Column<int>("int", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Blogs", x => x.Id); });

        migrationBuilder.CreateTable(
            "SliderInfos",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>("nvarchar(max)", nullable: false),
                Description = table.Column<string>("nvarchar(max)", nullable: false),
                SignImage = table.Column<string>("nvarchar(max)", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_SliderInfos", x => x.Id); });

        migrationBuilder.CreateTable(
            "Sliders",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Image = table.Column<string>("nvarchar(max)", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Sliders", x => x.Id); });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Blogs");

        migrationBuilder.DropTable(
            "SliderInfos");

        migrationBuilder.DropTable(
            "Sliders");
    }
}