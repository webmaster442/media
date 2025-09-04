using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiCacheEntries",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValidityInSeconds = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiCacheEntries", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "FolderBookmarks",
                columns: table => new
                {
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderBookmarks", x => x.Path);
                });

            migrationBuilder.CreateTable(
                name: "Metadata",
                columns: table => new
                {
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Artist = table.Column<string>(type: "TEXT", nullable: false),
                    Album = table.Column<string>(type: "TEXT", nullable: false),
                    Genre = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<uint>(type: "INTEGER", nullable: false),
                    Size = table.Column<long>(type: "INTEGER", nullable: false),
                    DiscNumber = table.Column<uint>(type: "INTEGER", nullable: false),
                    TrackNumber = table.Column<uint>(type: "INTEGER", nullable: false),
                    PlayTimeInSeconds = table.Column<double>(type: "REAL", nullable: false),
                    Codecs = table.Column<string>(type: "TEXT", nullable: false),
                    VideoWidth = table.Column<int>(type: "INTEGER", nullable: false),
                    VideoHeight = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadata", x => x.Path);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "PlayedEntries",
                columns: table => new
                {
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    LastPlayed = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetadataPath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayedEntries", x => x.Path);
                    table.ForeignKey(
                        name: "FK_PlayedEntries_Metadata_MetadataPath",
                        column: x => x.MetadataPath,
                        principalTable: "Metadata",
                        principalColumn: "Path");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_Album",
                table: "Metadata",
                column: "Album");

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_Artist",
                table: "Metadata",
                column: "Artist");

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_Genre",
                table: "Metadata",
                column: "Genre");

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_Size",
                table: "Metadata",
                column: "Size");

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_Year",
                table: "Metadata",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_PlayedEntries_MetadataPath",
                table: "PlayedEntries",
                column: "MetadataPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiCacheEntries");

            migrationBuilder.DropTable(
                name: "FolderBookmarks");

            migrationBuilder.DropTable(
                name: "PlayedEntries");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Metadata");
        }
    }
}
