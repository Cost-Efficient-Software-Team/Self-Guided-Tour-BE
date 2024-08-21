using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAverageRatingToTour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "Tours",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e6355698-6d3e-4dbe-a00f-3460ee378f0a", new DateTime(2024, 8, 12, 5, 31, 49, 802, DateTimeKind.Local).AddTicks(2490), "AQAAAAIAAYagAAAAEJVjOm3sfWsgrhOO7arBq8btJeuT/gix8BptRlpWyfbDnjtgGKPzsdtr8uRYz6T9Eg==", "1109a2cf-4f13-4239-bd2d-26fce588d523" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
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
