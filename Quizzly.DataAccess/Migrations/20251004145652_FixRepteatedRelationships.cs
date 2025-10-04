using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizzly.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixRepteatedRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Instructors_InstructorId1",
                table: "Quizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_QuizCategories_QuizCategoryId1",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_InstructorId1",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_QuizCategoryId1",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "InstructorId1",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuizCategoryId1",
                table: "Quizzes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstructorId1",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuizCategoryId1",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_InstructorId1",
                table: "Quizzes",
                column: "InstructorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_QuizCategoryId1",
                table: "Quizzes",
                column: "QuizCategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Instructors_InstructorId1",
                table: "Quizzes",
                column: "InstructorId1",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_QuizCategories_QuizCategoryId1",
                table: "Quizzes",
                column: "QuizCategoryId1",
                principalTable: "QuizCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
