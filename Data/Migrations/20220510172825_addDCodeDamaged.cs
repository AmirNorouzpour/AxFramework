using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addDCodeDamaged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DamageCode",
                table: "Damageds",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DamageCode",
                table: "Damageds");
        }
    }
}
