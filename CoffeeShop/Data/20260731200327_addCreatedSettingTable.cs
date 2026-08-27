#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace FiorelloBackendPractice.Data;

/// <inheritdoc />
public partial class addCreatedSettingTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Settings",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                HeaderLogo = table.Column<string>("nvarchar(max)", nullable: false),
                Phone = table.Column<string>("nvarchar(max)", nullable: false),
                Email = table.Column<string>("nvarchar(max)", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Settings", x => x.Id); });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Settings");
    }
}