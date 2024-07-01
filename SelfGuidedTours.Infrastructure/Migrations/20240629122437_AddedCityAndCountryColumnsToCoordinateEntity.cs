using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCityAndCountryColumnsToCoordinateEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Coordinates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Coordinates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bb889606-411a-43a7-9367-9966e88e2247", new DateTime(2024, 6, 29, 15, 24, 36, 114, DateTimeKind.Local).AddTicks(8627), "AQAAAAIAAYagAAAAEFGW5xzT5u8SWB17ORyqkvT80WbDbfIRh0vrYi78CPgWgEHRuRoSSguYvuCrMJWVNg==", "74ee551f-8585-4cab-8180-0c92b2b19b3a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Coordinates");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Coordinates");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c88d9339-d6b6-4ae2-9227-29d205b68e2d", new DateTime(2024, 6, 29, 15, 18, 5, 196, DateTimeKind.Local).AddTicks(9971), "AQAAAAIAAYagAAAAEMW/kCfCOgN9E5COHScM2IMGJTEb+LR500fcw9cwRMPpAdllGK0Q4WHU3L0U8bNmVA==", "2ab821c2-a43a-4924-ba68-eec96b82e724" });
        }
    }
}
