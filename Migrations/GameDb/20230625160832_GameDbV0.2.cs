using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeepoGuessrApi.Migrations.GameDb
{
    /// <inheritdoc />
    public partial class GameDbV02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsPromotionEnable",
                value: true);

            migrationBuilder.UpdateData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsPromotionEnable",
                value: true);

            migrationBuilder.UpdateData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsPromotionEnable",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsPromotionEnable",
                value: false);

            migrationBuilder.UpdateData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsPromotionEnable",
                value: false);

            migrationBuilder.UpdateData(
                table: "GameTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsPromotionEnable",
                value: false);
        }
    }
}
