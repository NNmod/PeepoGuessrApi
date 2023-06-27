using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeepoGuessrApi.Migrations.GameDb
{
    /// <inheritdoc />
    public partial class GameDbV03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GuessAvailable",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Health",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PosX",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PosY",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "Users",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "GuessAvailable",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Health",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PosX",
                table: "Users",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PosY",
                table: "Users",
                type: "double precision",
                nullable: true);
        }
    }
}
