using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Action" },
                    { 2, "Comedy" },
                    { 3, "Drama" }
                });

            migrationBuilder.InsertData(
                table: "Studios",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Studio A" },
                    { 2, "Studio B (IMAX)" },
                    { 3, "Studio C" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Description", "GenreId", "Rating", "ReleaseDate", "Title" },
                values: new object[,]
                {
                    { 1, "A masked vigilante fights crime in Gotham City.", 1, 9.0, new DateTime(2008, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Dark Knight" },
                    { 2, "The story of a man with a low IQ who accomplished great things.", 3, 8.8000000000000007, new DateTime(1994, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Forrest Gump" },
                    { 3, "Two co-dependent high school seniors are forced to deal with separation anxiety.", 2, 7.5999999999999996, new DateTime(2007, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Superbad" }
                });

            migrationBuilder.InsertData(
                table: "Schedules",
                columns: new[] { "Id", "MovieId", "Price", "ShowTime", "StudioId" },
                values: new object[,]
                {
                    { 1, 1, 50000m, new DateTime(2025, 8, 12, 19, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 1, 65000m, new DateTime(2025, 8, 12, 21, 30, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, 2, 45000m, new DateTime(2025, 8, 12, 19, 15, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 4, 3, 45000m, new DateTime(2025, 8, 13, 20, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Studios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Studios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Studios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
