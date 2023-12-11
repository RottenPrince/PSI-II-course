using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBoxAPI.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class renamedentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionSolveRunJoinModels");

            migrationBuilder.DropTable(
                name: "SolveRunModels");

            migrationBuilder.CreateTable(
                name: "QuizModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizModels_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestionRelationModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuizModelID = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionModelID = table.Column<int>(type: "INTEGER", nullable: false),
                    AnswerOptionModelID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestionRelationModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestionRelationModels_AnswerOptions_AnswerOptionModelID",
                        column: x => x.AnswerOptionModelID,
                        principalTable: "AnswerOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizQuestionRelationModels_Questions_QuestionModelID",
                        column: x => x.QuestionModelID,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizQuestionRelationModels_QuizModels_QuizModelID",
                        column: x => x.QuizModelID,
                        principalTable: "QuizModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizModels_RoomId",
                table: "QuizModels",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionRelationModels_AnswerOptionModelID",
                table: "QuizQuestionRelationModels",
                column: "AnswerOptionModelID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionRelationModels_QuestionModelID",
                table: "QuizQuestionRelationModels",
                column: "QuestionModelID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionRelationModels_QuizModelID",
                table: "QuizQuestionRelationModels",
                column: "QuizModelID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizQuestionRelationModels");

            migrationBuilder.DropTable(
                name: "QuizModels");

            migrationBuilder.CreateTable(
                name: "SolveRunModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolveRunModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolveRunModels_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionSolveRunJoinModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnswerOptionModelID = table.Column<int>(type: "INTEGER", nullable: true),
                    QuestionModelID = table.Column<int>(type: "INTEGER", nullable: false),
                    SolveRunModelID = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_SolveRunModels_RoomId",
                table: "SolveRunModels",
                column: "RoomId");
        }
    }
}
