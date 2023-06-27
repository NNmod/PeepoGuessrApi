using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeepoGuessrApi.Migrations
{
    /// <inheritdoc />
    public partial class AccountDbV03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuessAvailable",
                table: "RoundSummaries",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuessAvailable",
                table: "RoundSummaries");
        }
    }
}
