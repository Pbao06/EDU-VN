using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Source.Migrations
{
    /// <inheritdoc />
    public partial class AddCareerDetailFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "Careers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RequiredSkills",
                table: "Careers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Careers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Careers");

            migrationBuilder.DropColumn(
                name: "RequiredSkills",
                table: "Careers");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Careers");
        }
    }
}
