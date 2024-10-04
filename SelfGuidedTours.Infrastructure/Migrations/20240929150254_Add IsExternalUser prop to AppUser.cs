using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsExternalUserproptoAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExternalUser",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "IsExternalUser", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a9918312-f57c-483f-b8a1-653f57a98af9", new DateTime(2024, 9, 29, 18, 2, 53, 93, DateTimeKind.Local).AddTicks(1201), false, "AQAAAAIAAYagAAAAEDf2njz0BwoHWDu+EztO/9DyF0efY0/SapHFkIfHsafq7rYb03iAwgonC8sYEuZOaw==", "e99b0f0f-feaf-48c3-bfcb-2d5e5f5a3edb" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExternalUser",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eb65f6c4-a6f6-45c5-9ed9-5f83db148b5d", new DateTime(2024, 9, 18, 5, 30, 12, 337, DateTimeKind.Local).AddTicks(550), "AQAAAAIAAYagAAAAEGIrzSfGmoR0WeCm6CU8mW9PMsg2kuR4ZOKmjeQIN119ib+QFjcf7ka0buwad5voMw==", "be57d168-b9b8-4112-a286-b8aa795f3978" });
        }
    }
}
