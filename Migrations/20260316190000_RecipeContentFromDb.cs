using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApplication.Data;

#nullable disable

namespace WebApplication.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260316190000_RecipeContentFromDb")]
    public partial class RecipeContentFromDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cuisine",
                table: "Recipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IngredientsText",
                table: "Recipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Recipes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Recipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StepImagesFolder",
                table: "Recipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StepsText",
                table: "Recipes",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Cuisine", table: "Recipes");
            migrationBuilder.DropColumn(name: "IngredientsText", table: "Recipes");
            migrationBuilder.DropColumn(name: "IsFavorite", table: "Recipes");
            migrationBuilder.DropColumn(name: "Slug", table: "Recipes");
            migrationBuilder.DropColumn(name: "StepImagesFolder", table: "Recipes");
            migrationBuilder.DropColumn(name: "StepsText", table: "Recipes");
        }
    }
}
