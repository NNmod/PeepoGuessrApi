using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PeepoGuessrApi.Migrations
{
    /// <inheritdoc />
    public partial class AccountDbV05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Maps",
                columns: new[] { "Id", "IsClassic", "Name", "Url" },
                values: new object[,]
                {
                    { 3, false, "PPL5MP", "ppl5n" },
                    { 4, true, "PPL7MP1", "ppl7n1" },
                    { 5, true, "PPL7MP2", "ppl7n2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Maps",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Maps",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Maps",
                keyColumn: "Id",
                keyValue: 5);

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
    }
}
