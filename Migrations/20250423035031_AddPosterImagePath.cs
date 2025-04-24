using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StageCraft.Migrations
{
    /// <inheritdoc />
    public partial class AddPosterImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PosterImagePath",
                table: "Productions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterImagePath",
                table: "Productions");
        }
    }
}
