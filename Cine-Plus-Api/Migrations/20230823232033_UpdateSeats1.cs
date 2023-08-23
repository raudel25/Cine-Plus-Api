using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cine_Plus_Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeats1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Orders_OrderId",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_OrderId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Seats");

            migrationBuilder.CreateTable(
                name: "OrderSeat",
                columns: table => new
                {
                    OrdersId = table.Column<int>(type: "int", nullable: false),
                    SeatsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSeat", x => new { x.OrdersId, x.SeatsId });
                    table.ForeignKey(
                        name: "FK_OrderSeat_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderSeat_Seats_SeatsId",
                        column: x => x.SeatsId,
                        principalTable: "Seats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSeat_SeatsId",
                table: "OrderSeat",
                column: "SeatsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderSeat");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Seats",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_OrderId",
                table: "Seats",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Orders_OrderId",
                table: "Seats",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
