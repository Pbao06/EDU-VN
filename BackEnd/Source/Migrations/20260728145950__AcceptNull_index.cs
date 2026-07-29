using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Source.Migrations
{
    /// <inheritdoc />
    public partial class _AcceptNull_index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AsnwerIndex",
                table: "LearningAnswers",
                newName: "AnswerIndex");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AnswerIndex",
                table: "LearningAnswers",
                newName: "AsnwerIndex");
        }
    }
}
