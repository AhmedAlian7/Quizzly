using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizzly.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedAutoGradeToQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoGrade",
                table: "Questions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoGrade",
                table: "Questions");
        }
    }
}
