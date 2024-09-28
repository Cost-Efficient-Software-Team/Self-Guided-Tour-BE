using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeAverageRatingTo1stAfterDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "AverageRating",
                table: "Tours",
                type: "decimal(2,1)",
                nullable: false,
                comment: "Tour's Average Rating",
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)",
                oldComment: "Tour's Average Rating");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b20666fd-b202-4389-befc-6335b43180f7", new DateTime(2024, 9, 28, 13, 6, 41, 267, DateTimeKind.Local).AddTicks(3712), "AQAAAAIAAYagAAAAEM+ym7HcNvV8aCic3DHnDM/IRHRqseLZTrs0qWlfL2g6SwNblbtPRBUphEX2hLJe3Q==", "b0dc85c8-1284-4907-ad6e-c461cbff73c7" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "AverageRating",
                table: "Tours",
                type: "decimal(3,2)",
                nullable: false,
                comment: "Tour's Average Rating",
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)",
                oldComment: "Tour's Average Rating");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eb65f6c4-a6f6-45c5-9ed9-5f83db148b5d", new DateTime(2024, 9, 18, 5, 30, 12, 337, DateTimeKind.Local).AddTicks(550), "AQAAAAIAAYagAAAAEGIrzSfGmoR0WeCm6CU8mW9PMsg2kuR4ZOKmjeQIN119ib+QFjcf7ka0buwad5voMw==", "be57d168-b9b8-4112-a286-b8aa795f3978" });
        }
    }
}
