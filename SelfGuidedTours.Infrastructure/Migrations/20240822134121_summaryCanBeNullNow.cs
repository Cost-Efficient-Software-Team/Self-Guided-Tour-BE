using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class summaryCanBeNullNow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "Tours",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a0797ded-0bd5-4715-b486-6eb9833fd5b8", new DateTime(2024, 8, 22, 16, 41, 20, 622, DateTimeKind.Local).AddTicks(184), "AQAAAAIAAYagAAAAEG2XWx+wfkGjWq1zol5+7W1RY2B4KmcX9wpJ/KCsNxaGowDKLANKvap2BQUvLIksOQ==", "51a1a68a-a530-4caf-8f1a-1b9b9961117a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "Tours",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ee5b71d5-3af1-44f6-b4d6-fd68f6a3960a", new DateTime(2024, 8, 15, 11, 12, 33, 549, DateTimeKind.Local).AddTicks(7512), "AQAAAAIAAYagAAAAEOwdPeaWXYW4ySRQhGR0MkNWDhygpRIt3bSAiKlBfp3zJCCnebmtOwNbvD9dHKDRyw==", "6304ad3e-c51d-42a1-87ba-a8ebc4085707" });
        }
    }
}
