using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeepoGuessrApi.Migrations.GameDb
{
    /// <inheritdoc />
    public partial class GameDbV04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "RoundDuration",
                value: 180);

            migrationBuilder.UpdateData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "RoundDuration",
                value: 180);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "RoundDuration",
                value: 300);

            migrationBuilder.UpdateData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "RoundDuration",
                value: 300);
        }
    }
}
