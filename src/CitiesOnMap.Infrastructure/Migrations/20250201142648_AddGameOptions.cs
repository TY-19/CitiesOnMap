using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CitiesOnMap.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGameOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameOptionsId",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "GameOptions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShowCountry = table.Column<bool>(type: "bit", nullable: false),
                    ShowPopulation = table.Column<bool>(type: "bit", nullable: false),
                    CapitalsWithPopulationOver = table.Column<int>(type: "int", nullable: false),
                    CitiesWithPopulationOver = table.Column<int>(type: "int", nullable: false),
                    DistanceUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxPointForAnswer = table.Column<int>(type: "int", nullable: false),
                    ReducePointsPerUnit = table.Column<int>(type: "int", nullable: false),
                    AllowNegativePoints = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameOptions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameOptions_GameId",
                table: "GameOptions",
                column: "GameId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameOptions");

            migrationBuilder.DropColumn(
                name: "GameOptionsId",
                table: "Games");
        }
    }
}
