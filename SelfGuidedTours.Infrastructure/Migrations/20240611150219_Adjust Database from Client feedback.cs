using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfGuidedTours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustDatabasefromClientfeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourLandmarks");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Landmarks");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Landmarks");

            migrationBuilder.AddColumn<int>(
                name: "EstimatedDuration",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Estiamted duration in minutes");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailImageUrl",
                table: "Tours",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StopOrder",
                table: "Landmarks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TourId",
                table: "Landmarks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LandmarkResources",
                columns: table => new
                {
                    LandmarkResourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LandmarkId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandmarkResources", x => x.LandmarkResourceId);
                    table.ForeignKey(
                        name: "FK_LandmarkResources_Landmarks_LandmarkId",
                        column: x => x.LandmarkId,
                        principalTable: "Landmarks",
                        principalColumn: "LandmarkId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Landmarks_TourId",
                table: "Landmarks",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_LandmarkResources_LandmarkId",
                table: "LandmarkResources",
                column: "LandmarkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Landmarks_Tours_TourId",
                table: "Landmarks",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "TourId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Landmarks_Tours_TourId",
                table: "Landmarks");

            migrationBuilder.DropTable(
                name: "LandmarkResources");

            migrationBuilder.DropIndex(
                name: "IX_Landmarks_TourId",
                table: "Landmarks");

            migrationBuilder.DropColumn(
                name: "EstimatedDuration",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ThumbnailImageUrl",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "StopOrder",
                table: "Landmarks");

            migrationBuilder.DropColumn(
                name: "TourId",
                table: "Landmarks");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Landmarks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Landmarks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TourLandmarks",
                columns: table => new
                {
                    TourId = table.Column<int>(type: "int", nullable: false),
                    LandmarkId = table.Column<int>(type: "int", nullable: false),
                    StopOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourLandmarks", x => new { x.TourId, x.LandmarkId });
                    table.ForeignKey(
                        name: "FK_TourLandmarks_Landmarks_LandmarkId",
                        column: x => x.LandmarkId,
                        principalTable: "Landmarks",
                        principalColumn: "LandmarkId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourLandmarks_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourLandmarks_LandmarkId",
                table: "TourLandmarks",
                column: "LandmarkId");
        }
    }
}
