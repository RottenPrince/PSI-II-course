using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBoxAPI.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class addQuizToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "QuizModels",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_QuizModels_UserId",
                table: "QuizModels",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizModels_AspNetUsers_UserId",
                table: "QuizModels",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizModels_AspNetUsers_UserId",
                table: "QuizModels");

            migrationBuilder.DropIndex(
                name: "IX_QuizModels_UserId",
                table: "QuizModels");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QuizModels");
        }
    }
}
