using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentIntentIdToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ea9a2747-6f18-4e2e-ba35-53379dd27ab6", new DateTime(2024, 7, 13, 12, 2, 8, 279, DateTimeKind.Local).AddTicks(9069), "AQAAAAIAAYagAAAAEDIFmh2owHS12Rle3pUZioRPkcaIAfr1rW1/BV4nbkRgyEh4s07EQA37JOR1nR/bAA==", "b81b60be-431a-49fe-8f6a-b33b4e3f3d95" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3242ca29-7cd0-4787-b30a-23651e698c59", new DateTime(2024, 7, 8, 9, 57, 34, 4, DateTimeKind.Local).AddTicks(6660), "AQAAAAIAAYagAAAAEHWs9PMXTwrlRFgldpGnGk0H6Nue1uK0Zx1zb66izxKGUAplLC/wr1xAJgWfsMEDOg==", "ec8cf38e-7470-4fba-a0ab-b1a6bb72c0ff" });
        }
    }
}
