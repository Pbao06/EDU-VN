using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Source.Migrations
{
    /// <inheritdoc />
    public partial class fix_learningAnswerIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AsnwerIndex",
                table: "LearningUserAnswers");

            migrationBuilder.AddColumn<int>(
                name: "AsnwerIndex",
                table: "LearningAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AsnwerIndex",
                table: "LearningAnswers");

            migrationBuilder.AddColumn<int>(
                name: "AsnwerIndex",
                table: "LearningUserAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
