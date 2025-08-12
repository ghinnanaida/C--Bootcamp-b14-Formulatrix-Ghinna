using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateAndAddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Studios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    GenreId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Rating = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShowTime = table.Column<DateTime>(type: "TEXT", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    MovieId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schedules_Studios_StudioId",
                        column: x => x.StudioId,
                        principalTable: "Studios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Action" },
                    { 2, "Comedy" },
                    { 3, "Drama" },
                    { 4, "Sci-Fi" },
                    { 5, "Horror" },
                    { 6, "Romance" },
                    { 7, "Thriller" },
                    { 8, "Animation" }
                });

            migrationBuilder.InsertData(
                table: "Studios",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Studio 1" },
                    { 2, "Studio 2" },
                    { 3, "Studio 3 (IMAX)" },
                    { 4, "Studio 4 (Premiere)" },
                    { 5, "Studio 5" },
                    { 6, "Studio 6" },
                    { 7, "Studio 7" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Description", "GenreId", "Rating", "ReleaseDate", "Title" },
                values: new object[,]
                {
                    { 1, "An epic journey through space.", 4, 8.5, new DateTime(2023, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cosmic Voyager" },
                    { 2, "A comedian's final show.", 2, 7.7999999999999998, new DateTime(2022, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Last Laugh" },
                    { 3, "A historian discovers a secret.", 3, 8.9000000000000004, new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Echoes of Time" },
                    { 4, "A detective hunts a killer.", 7, 8.1999999999999993, new DateTime(2023, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Midnight Shadow" },
                    { 5, "Robots take over the world.", 1, 7.9000000000000004, new DateTime(2024, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cybernetic Dawn" },
                    { 6, "Something stirs in the woods.", 5, 7.5, new DateTime(2022, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Whispering Pines" },
                    { 7, "A love story in Paris.", 6, 8.0999999999999996, new DateTime(2023, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Parisian Heartbeat" },
                    { 8, "Giant robots fighting monsters.", 1, 7.0999999999999996, new DateTime(2022, 7, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Giga-Mech Smash" },
                    { 9, "A magical book comes to life.", 8, 8.5999999999999996, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Scribbler's Quest" },
                    { 10, "A submarine crew faces terror.", 7, 7.2999999999999998, new DateTime(2023, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Silent Depths" },
                    { 11, "An intergalactic tournament.", 4, 7.7000000000000002, new DateTime(2022, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Galactic Gladiators" },
                    { 12, "A family uncovers dark secrets.", 3, 8.4000000000000004, new DateTime(2024, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Inheritance" },
                    { 13, "Two book lovers find each other.", 6, 7.9000000000000004, new DateTime(2022, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Love in the Library" },
                    { 14, "A court jester's hilarious mishaps.", 2, 7.2000000000000002, new DateTime(2023, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jester's Folly" },
                    { 15, "A terrifying presence haunts a spaceship.", 5, 8.0, new DateTime(2024, 9, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Void Beckons" }
                });

            migrationBuilder.InsertData(
                table: "Schedules",
                columns: new[] { "Id", "MovieId", "Price", "ShowTime", "StudioId" },
                values: new object[,]
                {
                    { 1, 1, 45000m, new DateTime(2025, 8, 12, 13, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 2, 45000m, new DateTime(2025, 8, 12, 13, 30, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, 1, 45000m, new DateTime(2025, 8, 12, 15, 30, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, 4, 45000m, new DateTime(2025, 8, 12, 16, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 5, 5, 55000m, new DateTime(2025, 8, 12, 18, 45, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 6, 1, 55000m, new DateTime(2025, 8, 12, 19, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 7, 4, 55000m, new DateTime(2025, 8, 12, 19, 15, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 8, 5, 55000m, new DateTime(2025, 8, 12, 21, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 9, 6, 55000m, new DateTime(2025, 8, 12, 21, 30, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 10, 7, 55000m, new DateTime(2025, 8, 12, 21, 45, 0, 0, DateTimeKind.Unspecified), 7 },
                    { 11, 8, 45000m, new DateTime(2025, 8, 12, 14, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 12, 9, 45000m, new DateTime(2025, 8, 12, 16, 30, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 13, 8, 55000m, new DateTime(2025, 8, 12, 19, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 14, 10, 45000m, new DateTime(2025, 8, 13, 13, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 15, 11, 45000m, new DateTime(2025, 8, 13, 13, 30, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 16, 10, 45000m, new DateTime(2025, 8, 13, 15, 30, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 17, 12, 45000m, new DateTime(2025, 8, 13, 16, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 18, 13, 55000m, new DateTime(2025, 8, 13, 18, 45, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 19, 11, 55000m, new DateTime(2025, 8, 13, 19, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 20, 14, 55000m, new DateTime(2025, 8, 13, 19, 15, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 21, 13, 55000m, new DateTime(2025, 8, 13, 21, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 22, 15, 55000m, new DateTime(2025, 8, 13, 21, 30, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 23, 1, 55000m, new DateTime(2025, 8, 13, 21, 45, 0, 0, DateTimeKind.Unspecified), 7 },
                    { 24, 2, 45000m, new DateTime(2025, 8, 14, 13, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 25, 3, 45000m, new DateTime(2025, 8, 14, 13, 30, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 26, 2, 45000m, new DateTime(2025, 8, 14, 15, 30, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 27, 6, 45000m, new DateTime(2025, 8, 14, 16, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 28, 7, 55000m, new DateTime(2025, 8, 14, 18, 45, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 29, 9, 55000m, new DateTime(2025, 8, 14, 19, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 30, 12, 55000m, new DateTime(2025, 8, 14, 19, 15, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 31, 14, 55000m, new DateTime(2025, 8, 14, 21, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 32, 15, 55000m, new DateTime(2025, 8, 14, 21, 30, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 33, 1, 55000m, new DateTime(2025, 8, 14, 21, 45, 0, 0, DateTimeKind.Unspecified), 7 },
                    { 34, 11, 45000m, new DateTime(2025, 8, 14, 14, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 35, 5, 45000m, new DateTime(2025, 8, 14, 16, 30, 0, 0, DateTimeKind.Unspecified), 7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Id",
                table: "Genres",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_GenreId",
                table: "Movies",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Id",
                table: "Movies",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Id",
                table: "Schedules",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_MovieId",
                table: "Schedules",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_StudioId",
                table: "Schedules",
                column: "StudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Studios_Id",
                table: "Studios",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Studios");

            migrationBuilder.DropTable(
                name: "Genres");
        }
    }
}
