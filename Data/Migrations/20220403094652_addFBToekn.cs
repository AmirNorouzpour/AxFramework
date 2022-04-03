using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addFBToekn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FireBaseToken",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FireBaseToken",
                table: "Users");
        }
    }
}
