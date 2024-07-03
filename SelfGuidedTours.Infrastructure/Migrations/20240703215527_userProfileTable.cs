using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.UserId);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a7b6015d-87bc-4ec8-a030-23662c71d6c4", new DateTime(2024, 7, 4, 0, 55, 26, 579, DateTimeKind.Local).AddTicks(1943), "AQAAAAIAAYagAAAAEOswFRbfWYRU+ZLW+T+kJAA7FBcSpoDhxJLn9PUdGndbBkElyPylMKH77IHVJMTofg==", "943d7b20-66ee-408a-95be-62edcd64628e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bb889606-411a-43a7-9367-9966e88e2247", new DateTime(2024, 6, 29, 15, 24, 36, 114, DateTimeKind.Local).AddTicks(8627), "AQAAAAIAAYagAAAAEFGW5xzT5u8SWB17ORyqkvT80WbDbfIRh0vrYi78CPgWgEHRuRoSSguYvuCrMJWVNg==", "74ee551f-8585-4cab-8180-0c92b2b19b3a" });
        }
    }
}
