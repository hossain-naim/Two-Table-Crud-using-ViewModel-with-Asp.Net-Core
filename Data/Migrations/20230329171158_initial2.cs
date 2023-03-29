using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreNew2.Data.Migrations
{
    public partial class initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Players_playerlinkPlayerId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_playerlinkPlayerId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "playerlinkPlayerId",
                table: "Players");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "playerlinkPlayerId",
                table: "Players",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_playerlinkPlayerId",
                table: "Players",
                column: "playerlinkPlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Players_playerlinkPlayerId",
                table: "Players",
                column: "playerlinkPlayerId",
                principalTable: "Players",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
