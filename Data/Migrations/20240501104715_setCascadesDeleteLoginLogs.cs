using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class setCascadesDeleteLoginLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginLogs_Users_UserId",
                table: "LoginLogs");

            migrationBuilder.AddColumn<string>(
                name: "Number2",
                table: "UserMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LoginLogs_Users_UserId",
                table: "LoginLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginLogs_Users_UserId",
                table: "LoginLogs");

            migrationBuilder.DropColumn(
                name: "Number2",
                table: "UserMessages");

            migrationBuilder.AddForeignKey(
                name: "FK_LoginLogs_Users_UserId",
                table: "LoginLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
