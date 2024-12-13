using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchDogs.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class TradeModelModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProccessed",
                table: "Trades");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProccessed",
                table: "Trades",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
