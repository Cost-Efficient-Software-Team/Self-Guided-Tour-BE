using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHasPasswordfieldtoApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPassword",
                table: "AspNetUsers",
                type: "bit",
                nullable: true,
                comment: "External users dont have a password, they are authenticated by a third party.");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "HasPassword", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3803732e-b4ee-44ee-8977-2469e762cf4a", new DateTime(2024, 9, 15, 19, 22, 3, 419, DateTimeKind.Local).AddTicks(8405), true, "AQAAAAIAAYagAAAAEOSgybIfPWbYX6n5PmK1d0/X2qKuJA7YPKzRBEreb9cGztO0qt8SNDo9zq9jxNtwRg==", "23552ffa-dc7d-446e-934c-beb84508ba34" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPassword",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "174f63c9-bc10-4e5d-ad72-41573827a8a5", new DateTime(2024, 9, 8, 19, 53, 50, 198, DateTimeKind.Local).AddTicks(6070), "AQAAAAIAAYagAAAAEKoFu+TBj9d3V7fSTelY+wLXDpQKQYD48lyg6tARG2xrBYJTDchZyU5NLO7dOrVc2g==", "77ad568b-f44e-4420-a93a-e26e8316f5d8" });
        }
    }
}
