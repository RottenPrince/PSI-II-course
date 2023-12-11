using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBoxAPI.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class UnusedCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionModelSolveRunModel");

            migrationBuilder.DropColumn(
                name: "SolveRunJoinID",
                table: "Questions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SolveRunJoinID",
                table: "Questions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuestionModelSolveRunModel",
                columns: table => new
                {
                    QuestionRunId = table.Column<int>(type: "INTEGER", nullable: false),
                    SolveRunsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionModelSolveRunModel", x => new { x.QuestionRunId, x.SolveRunsId });
                    table.ForeignKey(
                        name: "FK_QuestionModelSolveRunModel_Questions_QuestionRunId",
                        column: x => x.QuestionRunId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionModelSolveRunModel_SolveRunModels_SolveRunsId",
                        column: x => x.SolveRunsId,
                        principalTable: "SolveRunModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionModelSolveRunModel_SolveRunsId",
                table: "QuestionModelSolveRunModel",
                column: "SolveRunsId");
        }
    }
}
