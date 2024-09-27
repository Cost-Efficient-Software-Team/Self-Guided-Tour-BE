using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilefields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Information about the current user.");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "Bio", "ConcurrencyStamp", "CreatedAt", "FirstName", "LastName", "PasswordHash", "ProfilePictureUrl", "SecurityStamp" },
                values: new object[] { null, "174f63c9-bc10-4e5d-ad72-41573827a8a5", new DateTime(2024, 9, 8, 19, 53, 50, 198, DateTimeKind.Local).AddTicks(6070), null, null, "AQAAAAIAAYagAAAAEKoFu+TBj9d3V7fSTelY+wLXDpQKQYD48lyg6tARG2xrBYJTDchZyU5NLO7dOrVc2g==", null, "77ad568b-f44e-4420-a93a-e26e8316f5d8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9c5f1ef0-b650-4ac4-915e-d2381a7d49d5", new DateTime(2024, 8, 23, 15, 29, 5, 240, DateTimeKind.Local).AddTicks(7335), "AQAAAAIAAYagAAAAENaZKympHG2VcQUp7gwu07NumyLr4wHHA6uF6fYZz2A7qergxqo9QDjnQ/KCn4WX/Q==", "437c2575-ada8-43f5-898a-b6001fc992a0" });
        }
    }
}
