using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class allownullsignalId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AxPositions_AxSignals_AxSignalId",
                table: "AxPositions");

            migrationBuilder.AlterColumn<int>(
                name: "AxSignalId",
                table: "AxPositions",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AxPositions_AxSignals_AxSignalId",
                table: "AxPositions",
                column: "AxSignalId",
                principalTable: "AxSignals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AxPositions_AxSignals_AxSignalId",
                table: "AxPositions");

            migrationBuilder.AlterColumn<int>(
                name: "AxSignalId",
                table: "AxPositions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AxPositions_AxSignals_AxSignalId",
                table: "AxPositions",
                column: "AxSignalId",
                principalTable: "AxSignals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
