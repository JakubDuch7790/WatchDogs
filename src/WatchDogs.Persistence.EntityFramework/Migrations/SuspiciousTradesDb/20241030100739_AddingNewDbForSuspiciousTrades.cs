using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchDogs.Persistence.EntityFramework.Migrations.SuspiciousTradesDb
{
    /// <inheritdoc />
    public partial class AddingNewDbForSuspiciousTrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SuspiciousTrades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsProccessed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuspiciousTrades", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SuspiciousTrades");
        }
    }
}
