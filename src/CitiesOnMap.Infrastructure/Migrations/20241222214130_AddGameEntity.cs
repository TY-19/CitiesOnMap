using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CitiesOnMap.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGameEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlayerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Previous = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentCityId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastPlayTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Cities_CurrentCityId",
                        column: x => x.CurrentCityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_CurrentCityId",
                table: "Games",
                column: "CurrentCityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
