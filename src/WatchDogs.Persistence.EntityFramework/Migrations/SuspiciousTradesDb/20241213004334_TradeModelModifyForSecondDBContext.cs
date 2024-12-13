using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchDogs.Persistence.EntityFramework.Migrations.SuspiciousTradesDb
{
    /// <inheritdoc />
    public partial class TradeModelModifyForSecondDBContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProccessed",
                table: "SuspiciousTrades");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProccessed",
                table: "SuspiciousTrades",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
