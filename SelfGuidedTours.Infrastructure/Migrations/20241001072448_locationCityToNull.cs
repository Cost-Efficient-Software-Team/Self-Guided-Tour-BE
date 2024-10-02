using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class locationCityToNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Coordinates",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Coordinate's City",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldComment: "Coordinate's City");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "58df4f2e-596c-4d93-b31b-2919617c4b0c", new DateTime(2024, 10, 1, 10, 24, 46, 975, DateTimeKind.Local).AddTicks(7285), "AQAAAAIAAYagAAAAEBFCFNyehSt7EivgYLcB062HusliTjfD1p4Qht+FFocOn0Wgxl5Cxts44H3UIyzdNQ==", "dabcaba9-d3f5-4adb-b6be-2d5638557dae" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Coordinates",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                comment: "Coordinate's City",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "Coordinate's City");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b20666fd-b202-4389-befc-6335b43180f7", new DateTime(2024, 9, 28, 13, 6, 41, 267, DateTimeKind.Local).AddTicks(3712), "AQAAAAIAAYagAAAAEM+ym7HcNvV8aCic3DHnDM/IRHRqseLZTrs0qWlfL2g6SwNblbtPRBUphEX2hLJe3Q==", "b0dc85c8-1284-4907-ad6e-c461cbff73c7" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
