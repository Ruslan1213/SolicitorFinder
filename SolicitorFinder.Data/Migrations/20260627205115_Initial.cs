using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolicitorFinder.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SolicitorAreaExternalId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UpdateInterval = table.Column<int>(type: "int", nullable: false),
                    AutoUpdate = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaxResults = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Solicitors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RatingStars = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    ReviewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ScrapedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ConfigEntityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_ConfigEntity_ConfigEntityId",
                        column: x => x.ConfigEntityId,
                        principalTable: "ConfigEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SolicitorAreas",
                columns: table => new
                {
                    SolicitorId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitorAreas", x => new { x.SolicitorId, x.AreaId });
                    table.ForeignKey(
                        name: "FK_SolicitorAreas_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitorAreas_Solicitors_SolicitorId",
                        column: x => x.SolicitorId,
                        principalTable: "Solicitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitorLocations",
                columns: table => new
                {
                    SolicitorId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitorLocations", x => new { x.SolicitorId, x.LocationId });
                    table.ForeignKey(
                        name: "FK_SolicitorLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitorLocations_Solicitors_SolicitorId",
                        column: x => x.SolicitorId,
                        principalTable: "Solicitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Area_ExternalId",
                table: "Areas",
                column: "SolicitorAreaExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_Name",
                table: "Areas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Location_Title_Text",
                table: "Locations",
                columns: new[] { "Title", "Text" });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ConfigEntityId",
                table: "Locations",
                column: "ConfigEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitorAreas_AreaId",
                table: "SolicitorAreas",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitorLocations_LocationId",
                table: "SolicitorLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitor_Name",
                table: "Solicitors",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitor_Phone",
                table: "Solicitors",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitor_Rating_Reviews",
                table: "Solicitors",
                columns: new[] { "RatingStars", "ReviewCount" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitorAreas");

            migrationBuilder.DropTable(
                name: "SolicitorLocations");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Solicitors");

            migrationBuilder.DropTable(
                name: "ConfigEntity");
        }
    }
}
