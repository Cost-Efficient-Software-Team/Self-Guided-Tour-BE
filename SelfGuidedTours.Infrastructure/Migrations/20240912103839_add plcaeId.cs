using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addplcaeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlaceId",
                table: "Landmarks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8f75e14f-7de8-451f-a79c-f8f1ef056f3f", new DateTime(2024, 9, 12, 13, 38, 36, 347, DateTimeKind.Local).AddTicks(3427), "AQAAAAIAAYagAAAAECJBHAmeB+BORjYUuELRTVwVI0vOOQV9wFCuFouGdrTq4bzvbNdOp0o49sJg5sy0Hg==", "b7d54279-fc3b-4b0a-aa16-caedb3f31604" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "Landmarks");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9c5f1ef0-b650-4ac4-915e-d2381a7d49d5", new DateTime(2024, 8, 23, 15, 29, 5, 240, DateTimeKind.Local).AddTicks(7335), "AQAAAAIAAYagAAAAENaZKympHG2VcQUp7gwu07NumyLr4wHHA6uF6fYZz2A7qergxqo9QDjnQ/KCn4WX/Q==", "437c2575-ada8-43f5-898a-b6001fc992a0" });
        }
    }
}
