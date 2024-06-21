using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tours_Coordinates_EndCoordinateId",
                table: "Tours");

            migrationBuilder.DropForeignKey(
                name: "FK_Tours_Coordinates_StartCoordinateId",
                table: "Tours");

            migrationBuilder.DropIndex(
                name: "IX_Tours_EndCoordinateId",
                table: "Tours");

            migrationBuilder.DropIndex(
                name: "IX_Tours_StartCoordinateId",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "EndCoordinateId",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "StartCoordinateId",
                table: "Tours");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndCoordinateId",
                table: "Tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartCoordinateId",
                table: "Tours",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tours_EndCoordinateId",
                table: "Tours",
                column: "EndCoordinateId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_StartCoordinateId",
                table: "Tours",
                column: "StartCoordinateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_Coordinates_EndCoordinateId",
                table: "Tours",
                column: "EndCoordinateId",
                principalTable: "Coordinates",
                principalColumn: "CoordinateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_Coordinates_StartCoordinateId",
                table: "Tours",
                column: "StartCoordinateId",
                principalTable: "Coordinates",
                principalColumn: "CoordinateId");
        }
    }
}
