using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddfieldsCreatedandUpdatedoneverytableandaddfieldLocationtoTours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Wallets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Wallets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserTours",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Transactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Tours",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Landmarks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Landmarks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LandmarkResources",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LandmarkResources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Coordinates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Coordinates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp", "UpdatedAt" },
                values: new object[] { "674a8bda-8a36-443e-802c-0bcfdf6f365e", new DateTime(2024, 6, 20, 15, 2, 38, 813, DateTimeKind.Local).AddTicks(5805), "AQAAAAIAAYagAAAAEOjhk4C5IBkMvkcLHDejzPkIzNfnOKvD49J1euS89mvtY2Sr6ApUo8Qht8FwKEUi+g==", "44695106-51bb-48a9-a131-1f934430734b", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserTours");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Landmarks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Landmarks");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LandmarkResources");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LandmarkResources");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Coordinates");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Coordinates");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "10646b30-6b2b-4225-9fa9-364d1c8846bd", new DateTime(2024, 6, 10, 22, 47, 6, 35, DateTimeKind.Local).AddTicks(1075), "AQAAAAIAAYagAAAAEJcTtrWUJTOR3Uqd+4bH0X9Hb+jGrF2bEzkCfNqvs8eIZuv3mY4vDitAU1liyZM2Tw==", "5f65bb9b-dde9-4edf-af27-94d9592a0767" });
        }
    }
}
