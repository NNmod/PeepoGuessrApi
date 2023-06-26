using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeepoGuessrApi.Migrations
{
    /// <inheritdoc />
    public partial class AccountDbV02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Maps",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsClassic",
                value: true);

            migrationBuilder.UpdateData(
                table: "Maps",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsClassic",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Maps",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsClassic",
                value: false);

            migrationBuilder.UpdateData(
                table: "Maps",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsClassic",
                value: false);
        }
    }
}
