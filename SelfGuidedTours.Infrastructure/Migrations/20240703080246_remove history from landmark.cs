using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removehistoryfromlandmark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "History",
                table: "Landmarks");

            migrationBuilder.AlterColumn<int>(
                name: "EstimatedDuration",
                table: "Tours",
                type: "int",
                nullable: false,
                comment: "Estimated duration in minutes",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Estiamted duration in minutes");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2e9a9cc6-ddb6-40dc-b2cc-e56c8cef7667", new DateTime(2024, 7, 3, 11, 2, 45, 249, DateTimeKind.Local).AddTicks(3684), "AQAAAAIAAYagAAAAEKKg16OjyjE+Qq/raVAjtOoO6fmjxoZ7oz1Zfa7XMC0McEbVInZNfW8dGJcBZob21g==", "1de572ff-0456-425a-a8ce-ccab86affb42" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EstimatedDuration",
                table: "Tours",
                type: "int",
                nullable: false,
                comment: "Estiamted duration in minutes",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Estimated duration in minutes");

            migrationBuilder.AddColumn<string>(
                name: "History",
                table: "Landmarks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a9f45f15-38da-45ab-847f-d8fb39efcf46", new DateTime(2024, 6, 20, 16, 15, 9, 768, DateTimeKind.Local).AddTicks(662), "AQAAAAIAAYagAAAAEM/+Q0fZrT1fdvcJgCkgk0nRGb8R2WZEPy/ap/SPmBWdnVcEYUda3u5P92keUnwE2Q==", "07ea68f3-45cb-4661-b834-010c5e75d8a1" });
        }
    }
}
