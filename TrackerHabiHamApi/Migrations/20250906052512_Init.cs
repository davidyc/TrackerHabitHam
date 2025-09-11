using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackerHabiHamApi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MounthWeights",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Weight = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MounthWeights", x => x.Date);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MounthWeights");
        }
    }
}


