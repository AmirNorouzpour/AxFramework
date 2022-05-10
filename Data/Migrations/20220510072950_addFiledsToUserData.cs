using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addFiledsToUserData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "UserData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "TradeId",
                table: "AxPositionLogs",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "TradeId",
                table: "AxPositionLogs");
        }
    }
}
