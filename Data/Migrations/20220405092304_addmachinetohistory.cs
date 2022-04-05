using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addmachinetohistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductInstanceHistories_OperationStations_OpId",
                table: "ProductInstanceHistories");

            migrationBuilder.DropIndex(
                name: "IX_ProductInstanceHistories_OpId",
                table: "ProductInstanceHistories");

            migrationBuilder.DropColumn(
                name: "OpId",
                table: "ProductInstanceHistories");

            migrationBuilder.AddColumn<int>(
                name: "MachineId",
                table: "ProductInstanceHistories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductInstanceHistories_MachineId",
                table: "ProductInstanceHistories",
                column: "MachineId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInstanceHistories_Machines_MachineId",
                table: "ProductInstanceHistories",
                column: "MachineId",
                principalTable: "Machines",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductInstanceHistories_Machines_MachineId",
                table: "ProductInstanceHistories");

            migrationBuilder.DropIndex(
                name: "IX_ProductInstanceHistories_MachineId",
                table: "ProductInstanceHistories");

            migrationBuilder.DropColumn(
                name: "MachineId",
                table: "ProductInstanceHistories");

            migrationBuilder.AddColumn<int>(
                name: "OpId",
                table: "ProductInstanceHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductInstanceHistories_OpId",
                table: "ProductInstanceHistories",
                column: "OpId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInstanceHistories_OperationStations_OpId",
                table: "ProductInstanceHistories",
                column: "OpId",
                principalTable: "OperationStations",
                principalColumn: "Id");
        }
    }
}
