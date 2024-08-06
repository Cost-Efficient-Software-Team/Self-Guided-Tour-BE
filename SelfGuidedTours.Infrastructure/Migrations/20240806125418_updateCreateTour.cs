using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateCreateTour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeTour",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "67659cba-5ced-4e6e-9988-a6e8f6dde2ff", new DateTime(2024, 8, 6, 15, 54, 17, 3, DateTimeKind.Local).AddTicks(3558), "AQAAAAIAAYagAAAAEN4UCvY56pQJwsbbFpyI4+jQrM3EPapk2TW6i56BT9El4lDYp1D2Qva0qNpRpcwaVw==", "7466c95e-d9b8-4941-bec0-c9371dba794f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeTour",
                table: "Tours");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a7b6015d-87bc-4ec8-a030-23662c71d6c4", new DateTime(2024, 7, 4, 0, 55, 26, 579, DateTimeKind.Local).AddTicks(1943), "AQAAAAIAAYagAAAAEOswFRbfWYRU+ZLW+T+kJAA7FBcSpoDhxJLn9PUdGndbBkElyPylMKH77IHVJMTofg==", "943d7b20-66ee-408a-95be-62edcd64628e" });
        }
    }
}
