using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addAxPositionsLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AxPositionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Symbol = table.Column<string>(nullable: true),
                    EnterAveragePrice = table.Column<decimal>(nullable: false),
                    ExitAveragePrice = table.Column<decimal>(nullable: false),
                    TempProfit = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    Side = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Profit = table.Column<decimal>(nullable: false),
                    ProfitPercent = table.Column<decimal>(nullable: false),
                    Commission = table.Column<decimal>(nullable: false),
                    InProgress = table.Column<bool>(nullable: false),
                    EnterTime = table.Column<DateTime>(nullable: false),
                    ExitTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AxPositionLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AxPositionLogs");
        }
    }
}
