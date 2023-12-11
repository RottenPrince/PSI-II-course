using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBoxAPI.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class WorkingSolveRun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SolveRunJoinID",
                table: "Questions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuestionSolveRunJoinModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SolveRunModelID = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionModelID = table.Column<int>(type: "INTEGER", nullable: false),
                    AnswerOptionModelID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSolveRunJoinModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionSolveRunJoinModels_AnswerOptions_AnswerOptionModelID",
                        column: x => x.AnswerOptionModelID,
                        principalTable: "AnswerOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionSolveRunJoinModels_Questions_QuestionModelID",
                        column: x => x.QuestionModelID,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionSolveRunJoinModels_SolveRunModels_SolveRunModelID",
                        column: x => x.SolveRunModelID,
                        principalTable: "SolveRunModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSolveRunJoinModels_AnswerOptionModelID",
                table: "QuestionSolveRunJoinModels",
                column: "AnswerOptionModelID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSolveRunJoinModels_QuestionModelID",
                table: "QuestionSolveRunJoinModels",
                column: "QuestionModelID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSolveRunJoinModels_SolveRunModelID",
                table: "QuestionSolveRunJoinModels",
                column: "SolveRunModelID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionSolveRunJoinModels");

            migrationBuilder.DropColumn(
                name: "SolveRunJoinID",
                table: "Questions");
        }
    }
}
