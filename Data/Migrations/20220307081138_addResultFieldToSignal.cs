using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addResultFieldToSignal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Result",
                table: "AxPositions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "AxPositions");
        }
    }
}
