using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    ImageFileName = table.Column<string>(type: "text", nullable: true),
                    CookingTime = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayName", "Name" },
                values: new object[,]
                {
                    { 1, "Русская кухня", "Russian" },
                    { 2, "Европейская кухня", "European" },
                    { 3, "Азиатская кухня", "Asian" }
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "CategoryId", "CookingTime", "CreatedAt", "Description", "Difficulty", "ImageFileName", "Name" },
                values: new object[,]
                {
                    { 1, 1, 20, new DateTime(2026, 2, 20, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5042), "Традиционный русский завтрак из творога", 1, "syrniki.png", "Сырники" },
                    { 2, 1, 60, new DateTime(2026, 2, 22, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5051), "Грузинский суп с говядиной и рисом", 2, "kharcho.png", "Харчо" },
                    { 3, 1, 40, new DateTime(2026, 2, 25, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5052), "Ароматное блюдо из лесных грибов", 2, "mushrooms.png", "Грибы в сметане" },
                    { 4, 1, 90, new DateTime(2026, 2, 23, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5054), "Украинский свёкольный суп с говядиной", 2, "borsch.png", "Борщ" },
                    { 5, 1, 30, new DateTime(2026, 2, 27, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5056), "Классический русский салат", 1, "olivie.png", "Оливье" },
                    { 6, 2, 15, new DateTime(2026, 2, 28, 16, 23, 43, 67, DateTimeKind.Utc).AddTicks(5057), "Свежий салат с помидорами, огурцами и фетой", 1, "greece_salad.png", "Греческий салат" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CategoryId",
                table: "Recipes",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
