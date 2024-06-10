using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Moveentityconfigurationsoutsideofcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Credentials", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "48cdb4f9-8275-4ad5-a00a-cb35e07b35f5", 0, "f2ee107d-a4ca-4884-a550-6ffbec31a852", new DateTime(2024, 6, 10, 22, 35, 18, 575, DateTimeKind.Local).AddTicks(9453), null, "ApplicationUser", "admin@selfguidedtours.bg", false, false, null, "Admin Adminov", "ADMIN@SELFGUIDEDTOURS.BG", "ADMIN ADMINOV", "AQAAAAIAAYagAAAAEJhvdLPnGkrjoTgojAhkqkugZvxBAK940X5z6wEkundShBn+9u0/mFwf7HqH0WR5NA==", null, false, "1d568b2b-6af2-46e2-b51d-7700c74060c8", false, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "48cdb4f9-8275-4ad5-a00a-cb35e07b35f5");
        }
    }
}
