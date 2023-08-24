using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cine_Plus_Api.Migrations
{
    /// <inheritdoc />
    public partial class Points2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pays_Users_UserId1",
                table: "Pays");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "Pays",
                newName: "TicketPointsUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Pays_UserId1",
                table: "Pays",
                newName: "IX_Pays_TicketPointsUser_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pays_Users_TicketPointsUser_UserId",
                table: "Pays",
                column: "TicketPointsUser_UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pays_Users_TicketPointsUser_UserId",
                table: "Pays");

            migrationBuilder.RenameColumn(
                name: "TicketPointsUser_UserId",
                table: "Pays",
                newName: "UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_Pays_TicketPointsUser_UserId",
                table: "Pays",
                newName: "IX_Pays_UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Pays_Users_UserId1",
                table: "Pays",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
