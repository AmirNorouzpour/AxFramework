using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addReachedTragetCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReachedTarget",
                table: "AxPositions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReachedTarget",
                table: "AxPositions");
        }
    }
}
