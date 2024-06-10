using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersRolesrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "48cdb4f9-8275-4ad5-a00a-cb35e07b35f5");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Credentials", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "27d78708-8671-4b05-bd5e-17aa91392224", 0, "10646b30-6b2b-4225-9fa9-364d1c8846bd", new DateTime(2024, 6, 10, 22, 47, 6, 35, DateTimeKind.Local).AddTicks(1075), null, "ApplicationUser", "admin@selfguidedtours.bg", false, false, null, "Admin Adminov", "ADMIN@SELFGUIDEDTOURS.BG", "ADMIN ADMINOV", "AQAAAAIAAYagAAAAEJcTtrWUJTOR3Uqd+4bH0X9Hb+jGrF2bEzkCfNqvs8eIZuv3mY4vDitAU1liyZM2Tw==", null, false, "5f65bb9b-dde9-4edf-af27-94d9592a0767", false, null });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "656a6079-ec9a-4a98-a484-2d1752156d60", "27d78708-8671-4b05-bd5e-17aa91392224" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "656a6079-ec9a-4a98-a484-2d1752156d60", "27d78708-8671-4b05-bd5e-17aa91392224" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "27d78708-8671-4b05-bd5e-17aa91392224");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Credentials", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "48cdb4f9-8275-4ad5-a00a-cb35e07b35f5", 0, "f2ee107d-a4ca-4884-a550-6ffbec31a852", new DateTime(2024, 6, 10, 22, 35, 18, 575, DateTimeKind.Local).AddTicks(9453), null, "ApplicationUser", "admin@selfguidedtours.bg", false, false, null, "Admin Adminov", "ADMIN@SELFGUIDEDTOURS.BG", "ADMIN ADMINOV", "AQAAAAIAAYagAAAAEJhvdLPnGkrjoTgojAhkqkugZvxBAK940X5z6wEkundShBn+9u0/mFwf7HqH0WR5NA==", null, false, "1d568b2b-6af2-46e2-b51d-7700c74060c8", false, null });
        }
    }
}
