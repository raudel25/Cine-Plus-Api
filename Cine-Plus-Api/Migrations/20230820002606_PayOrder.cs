using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cine_Plus_Api.Migrations
{
    /// <inheritdoc />
    public partial class PayOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaidSeatId",
                table: "Discounts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PayOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Price = table.Column<double>(type: "double", nullable: false),
                    Paid = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayOrders", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaidSeats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Price = table.Column<double>(type: "double", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    PayOrderId = table.Column<int>(type: "int", nullable: false),
                    ShowMovieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaidSeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaidSeats_PayOrders_PayOrderId",
                        column: x => x.PayOrderId,
                        principalTable: "PayOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaidSeats_ShowMovies_ShowMovieId",
                        column: x => x.ShowMovieId,
                        principalTable: "ShowMovies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_PaidSeatId",
                table: "Discounts",
                column: "PaidSeatId");

            migrationBuilder.CreateIndex(
                name: "IX_PaidSeats_PayOrderId",
                table: "PaidSeats",
                column: "PayOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaidSeats_ShowMovieId",
                table: "PaidSeats",
                column: "ShowMovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_PaidSeats_PaidSeatId",
                table: "Discounts",
                column: "PaidSeatId",
                principalTable: "PaidSeats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_PaidSeats_PaidSeatId",
                table: "Discounts");

            migrationBuilder.DropTable(
                name: "PaidSeats");

            migrationBuilder.DropTable(
                name: "PayOrders");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_PaidSeatId",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "PaidSeatId",
                table: "Discounts");
        }
    }
}
