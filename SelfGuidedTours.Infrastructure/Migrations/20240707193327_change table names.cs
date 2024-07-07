using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changetablenames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Tours");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Tours",
                newName: "Summary");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "Tours",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "81d9bf75-8194-4fc7-bfe7-d80fddb4e405", new DateTime(2024, 7, 7, 22, 33, 25, 998, DateTimeKind.Local).AddTicks(8956), "AQAAAAIAAYagAAAAEHDvqswVHp9+h68iTd3h2JbtlmOQjPssLqpHtI2TmFwv8rJBTKmZRHKbL+SA/mbiGA==", "187e4505-cd19-449e-81db-c650049cd07d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Tours");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "Tours",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Tours",
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
    }
}
