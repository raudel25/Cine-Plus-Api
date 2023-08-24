using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cine_Plus_Api.Migrations
{
    /// <inheritdoc />
    public partial class Points : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AddPoints",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PricePoints",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicketPointsUser_EmployId",
                table: "Pays",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Pays",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pays_TicketPointsUser_EmployId",
                table: "Pays",
                column: "TicketPointsUser_EmployId");

            migrationBuilder.CreateIndex(
                name: "IX_Pays_UserId1",
                table: "Pays",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Pays_Employs_TicketPointsUser_EmployId",
                table: "Pays",
                column: "TicketPointsUser_EmployId",
                principalTable: "Employs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pays_Users_UserId1",
                table: "Pays",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pays_Employs_TicketPointsUser_EmployId",
                table: "Pays");

            migrationBuilder.DropForeignKey(
                name: "FK_Pays_Users_UserId1",
                table: "Pays");

            migrationBuilder.DropIndex(
                name: "IX_Pays_TicketPointsUser_EmployId",
                table: "Pays");

            migrationBuilder.DropIndex(
                name: "IX_Pays_UserId1",
                table: "Pays");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddPoints",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "PricePoints",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "TicketPointsUser_EmployId",
                table: "Pays");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Pays");
        }
    }
}
