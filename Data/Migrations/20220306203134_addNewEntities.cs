using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addNewEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisMsgs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Title = table.Column<bool>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    Side = table.Column<string>(nullable: true),
                    Views = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisMsgs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisMsgs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AxSignals",
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
                    Side = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    TimeFrame = table.Column<string>(nullable: true),
                    EnterPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AxSignals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AxUserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SignalNotify = table.Column<bool>(nullable: false),
                    NewsNotify = table.Column<bool>(nullable: false),
                    AnalysisNotify = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AxUserSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Duration = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TransactionId = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AxPositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    AxSignalId = table.Column<int>(nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    EnterPrice = table.Column<decimal>(nullable: false),
                    StopLoss = table.Column<decimal>(nullable: false),
                    Targets = table.Column<string>(nullable: true),
                    Max = table.Column<decimal>(nullable: false),
                    ProfitPercent = table.Column<decimal>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    TimeFrame = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Risk = table.Column<string>(nullable: true),
                    Capital = table.Column<string>(nullable: true),
                    Leverage = table.Column<decimal>(nullable: false),
                    SuggestedLeverage = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IsFree = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AxPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AxPositions_AxSignals_AxSignalId",
                        column: x => x.AxSignalId,
                        principalTable: "AxSignals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisMsgs_UserId",
                table: "AnalysisMsgs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AxPositions_AxSignalId",
                table: "AxPositions",
                column: "AxSignalId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisMsgs");

            migrationBuilder.DropTable(
                name: "AxPositions");

            migrationBuilder.DropTable(
                name: "AxUserSettings");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "AxSignals");
        }
    }
}
