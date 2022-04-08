using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class DamagedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Damageds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ProductInstanceId = table.Column<int>(nullable: false),
                    PersonnelId = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Damageds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Damageds_Personnels_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Damageds_ProductInstances_ProductInstanceId",
                        column: x => x.ProductInstanceId,
                        principalTable: "ProductInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Damageds_PersonnelId",
                table: "Damageds",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_Damageds_ProductInstanceId",
                table: "Damageds",
                column: "ProductInstanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Damageds");
        }
    }
}
