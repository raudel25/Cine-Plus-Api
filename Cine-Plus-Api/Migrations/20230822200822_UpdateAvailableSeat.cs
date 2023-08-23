using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cine_Plus_Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAvailableSeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "AvailableSeats");

            migrationBuilder.AddColumn<DateTime>(
                name: "RowVersion",
                table: "AvailableSeats",
                type: "datetime(6)",
                rowVersion: true,
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AvailableSeats");

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "AvailableSeats",
                type: "longblob",
                nullable: false);
        }
    }
}
