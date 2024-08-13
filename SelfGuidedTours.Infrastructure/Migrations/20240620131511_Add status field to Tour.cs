using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddstatusfieldtoTour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "On create, status is UnderReview until approved or rejected by admin.");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a9f45f15-38da-45ab-847f-d8fb39efcf46", new DateTime(2024, 6, 20, 16, 15, 9, 768, DateTimeKind.Local).AddTicks(662), "AQAAAAIAAYagAAAAEM/+Q0fZrT1fdvcJgCkgk0nRGb8R2WZEPy/ap/SPmBWdnVcEYUda3u5P92keUnwE2Q==", "07ea68f3-45cb-4661-b834-010c5e75d8a1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tours");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "674a8bda-8a36-443e-802c-0bcfdf6f365e", new DateTime(2024, 6, 20, 15, 2, 38, 813, DateTimeKind.Local).AddTicks(5805), "AQAAAAIAAYagAAAAEOjhk4C5IBkMvkcLHDejzPkIzNfnOKvD49J1euS89mvtY2Sr6ApUo8Qht8FwKEUi+g==", "44695106-51bb-48a9-a131-1f934430734b" });
        }
    }
}
