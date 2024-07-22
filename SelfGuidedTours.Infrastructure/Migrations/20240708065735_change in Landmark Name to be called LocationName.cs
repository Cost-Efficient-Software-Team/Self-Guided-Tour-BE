using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeinLandmarkNametobecalledLocationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Landmarks",
                newName: "LocationName");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3242ca29-7cd0-4787-b30a-23651e698c59", new DateTime(2024, 7, 8, 9, 57, 34, 4, DateTimeKind.Local).AddTicks(6660), "AQAAAAIAAYagAAAAEHWs9PMXTwrlRFgldpGnGk0H6Nue1uK0Zx1zb66izxKGUAplLC/wr1xAJgWfsMEDOg==", "ec8cf38e-7470-4fba-a0ab-b1a6bb72c0ff" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocationName",
                table: "Landmarks",
                newName: "Name");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "81d9bf75-8194-4fc7-bfe7-d80fddb4e405", new DateTime(2024, 7, 7, 22, 33, 25, 998, DateTimeKind.Local).AddTicks(8956), "AQAAAAIAAYagAAAAEHDvqswVHp9+h68iTd3h2JbtlmOQjPssLqpHtI2TmFwv8rJBTKmZRHKbL+SA/mbiGA==", "187e4505-cd19-449e-81db-c650049cd07d" });
        }
    }
}
