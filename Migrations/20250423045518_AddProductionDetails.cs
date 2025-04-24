using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StageCraft.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Productions",
                newName: "ShowTimeEve");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingDay",
                table: "Productions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsWorldPremiere",
                table: "Productions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpeningDay",
                table: "Productions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Playwright",
                table: "Productions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Runtime",
                table: "Productions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "Productions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShowTimeMat",
                table: "Productions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketLink",
                table: "Productions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingDay",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "IsWorldPremiere",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "OpeningDay",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "Playwright",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "Runtime",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "ShowTimeMat",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "TicketLink",
                table: "Productions");

            migrationBuilder.RenameColumn(
                name: "ShowTimeEve",
                table: "Productions",
                newName: "Date");
        }
    }
}
