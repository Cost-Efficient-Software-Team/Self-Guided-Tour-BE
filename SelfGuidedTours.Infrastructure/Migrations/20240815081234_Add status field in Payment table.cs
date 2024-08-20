using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddstatusfieldinPaymenttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ee5b71d5-3af1-44f6-b4d6-fd68f6a3960a", new DateTime(2024, 8, 15, 11, 12, 33, 549, DateTimeKind.Local).AddTicks(7512), "AQAAAAIAAYagAAAAEOwdPeaWXYW4ySRQhGR0MkNWDhygpRIt3bSAiKlBfp3zJCCnebmtOwNbvD9dHKDRyw==", "6304ad3e-c51d-42a1-87ba-a8ebc4085707" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bb536c99-ccf7-49f8-bc11-afac48d98097", new DateTime(2024, 8, 14, 17, 44, 54, 144, DateTimeKind.Local).AddTicks(8595), "AQAAAAIAAYagAAAAEGz1RIhj8HQ0JnO9NfoD2QRhC4nadnBCra2nZZvjdCHwu4UG3xaFpBcBmktpimV8sQ==", "54e3bfdc-92c8-44c6-b612-08482e2fd5b5" });
        }
    }
}
