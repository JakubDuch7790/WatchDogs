using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchDogs.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SuspiciousTradesTableadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Trades",
                table: "Trades");

            migrationBuilder.RenameTable(
                name: "Trades",
                newName: "Trade");

            migrationBuilder.AddColumn<bool>(
                name: "IsProccessed",
                table: "Trade",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trade",
                table: "Trade",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Trade",
                table: "Trade");

            migrationBuilder.DropColumn(
                name: "IsProccessed",
                table: "Trade");

            migrationBuilder.RenameTable(
                name: "Trade",
                newName: "Trades");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trades",
                table: "Trades",
                column: "Id");
        }
    }
}
