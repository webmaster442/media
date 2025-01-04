using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.Database.Migrations
{
    /// <inheritdoc />
    public partial class Folderbookmarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolderBookmarks");
        }
    }
}
