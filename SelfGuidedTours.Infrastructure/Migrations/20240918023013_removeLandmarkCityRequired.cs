using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeLandmarkCityRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Coordinates",
                type: "nvarchar(500)",
                maxLength: 500,
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
                values: new object[] { "0fe912d3-0243-47ca-802c-49888bfa9a04", new DateTime(2024, 9, 18, 5, 30, 12, 337, DateTimeKind.Local).AddTicks(550), "AQAAAAIAAYagAAAAEIEbm7KxGHvdoirod73NXdJaBhn8CCM82SdRlMeOm1S6RfbnmCYvINKnBSsU0Rb3NA==", "77e47a5f-6db8-4678-a349-74ba7200efb1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Coordinates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ec61339d-789f-4655-b807-86bd64997e4d", new DateTime(2024, 9, 12, 22, 15, 37, 617, DateTimeKind.Local).AddTicks(5171), "AQAAAAIAAYagAAAAEKGlIdh+Zf0fpmari/kGmqPYDs0cL6xOsouSr3S/wLoPGDFW4dVpmL+Akfw2VWVFUA==", "f7472e89-45b1-423a-8c3a-4eca8f402818" });
        }
    }
}
