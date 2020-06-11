using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class TypoInPropertyNameOfMenuItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Spicyness",
                table: "MenuItem",
                newName: "Spiciness");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Spiciness",
                table: "MenuItem",
                newName: "Spicyness");
        }
    }
}
