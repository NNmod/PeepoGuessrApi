using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PeepoGuessrApi.Migrations.MaintenanceDb
{
    /// <inheritdoc />
    public partial class MaintenanceDbV02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Works",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Expire = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Works", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Accesses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Expire",
                value: new DateTime(2023, 7, 4, 12, 51, 14, 807, DateTimeKind.Utc).AddTicks(5574));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Works");

            migrationBuilder.UpdateData(
                table: "Accesses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Expire",
                value: new DateTime(2023, 6, 18, 23, 38, 7, 461, DateTimeKind.Utc).AddTicks(2541));
        }
    }
}
