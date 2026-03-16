using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class RatingAndNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                table: "Recipes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RatingSum",
                table: "Recipes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    ImageFileName = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "News",
                columns: new[] { "Id", "CreatedAt", "ImageFileName", "Summary", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 7, 0, 0, 0, 0, DateTimeKind.Utc), "news1.png", "В России предлагают ввести налог на вредные продукты питания.", "Налог на чипсы и газировку" },
                    { 2, new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), "news2.png", "Учёные нашли фрукт, который помогает быстрее восстановиться.", "Фрукт против похмелья" },
                    { 3, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "news3.png", "Исследователи признали безопасным только один вид подсластителя.", "Подсластители под угрозой" },
                    { 4, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), "news4.jpg", "Читайте о традициях и вкусах грузинской кухни.", "История грузинской кухни" }
                });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "RatingCount", "RatingSum" },
                values: new object[] { new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), 0, 0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "RatingCount", "RatingSum" },
                values: new object[] { new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Utc), 0, 0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "RatingCount", "RatingSum" },
                values: new object[] { new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), 0, 0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "RatingCount", "RatingSum" },
                values: new object[] { new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc), 0, 0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "RatingCount", "RatingSum" },
                values: new object[] { new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Utc), 0, 0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "RatingCount", "RatingSum" },
                values: new object[] { new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), 0, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RatingSum",
                table: "Recipes");

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 20, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5042));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 22, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5051));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 25, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5052));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 23, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5054));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 27, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5056));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 28, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5057));
        }
    }
}
