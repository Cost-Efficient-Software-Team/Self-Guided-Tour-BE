using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddstripecustomeridtoApplicationuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Id of the stripe customer associated with the user. Created when the user makes a payment.");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp", "StripeCustomerId" },
                values: new object[] { "bb536c99-ccf7-49f8-bc11-afac48d98097", new DateTime(2024, 8, 14, 17, 44, 54, 144, DateTimeKind.Local).AddTicks(8595), "AQAAAAIAAYagAAAAEGz1RIhj8HQ0JnO9NfoD2QRhC4nadnBCra2nZZvjdCHwu4UG3xaFpBcBmktpimV8sQ==", "54e3bfdc-92c8-44c6-b612-08482e2fd5b5", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a7b6015d-87bc-4ec8-a030-23662c71d6c4", new DateTime(2024, 7, 4, 0, 55, 26, 579, DateTimeKind.Local).AddTicks(1943), "AQAAAAIAAYagAAAAEOswFRbfWYRU+ZLW+T+kJAA7FBcSpoDhxJLn9PUdGndbBkElyPylMKH77IHVJMTofg==", "943d7b20-66ee-408a-95be-62edcd64628e" });
        }
    }
}
