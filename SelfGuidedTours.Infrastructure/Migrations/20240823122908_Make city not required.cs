using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Makecitynotrequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Coordinates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9c5f1ef0-b650-4ac4-915e-d2381a7d49d5", new DateTime(2024, 8, 23, 15, 29, 5, 240, DateTimeKind.Local).AddTicks(7335), "AQAAAAIAAYagAAAAENaZKympHG2VcQUp7gwu07NumyLr4wHHA6uF6fYZz2A7qergxqo9QDjnQ/KCn4WX/Q==", "437c2575-ada8-43f5-898a-b6001fc992a0" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Coordinates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6094ee2b-1146-43dd-a5b8-f25fabd60ac7", new DateTime(2024, 8, 22, 22, 12, 51, 882, DateTimeKind.Local).AddTicks(846), "AQAAAAIAAYagAAAAEKufWq1jzityLrl4t1Gh8vjtKrZmpwqlNb3Kar3rkC2ZUphSx0XVJ4F6kiy23vZ+hg==", "40984865-63c7-4e1d-a74c-198cec7ed918" });
        }
    }
}
