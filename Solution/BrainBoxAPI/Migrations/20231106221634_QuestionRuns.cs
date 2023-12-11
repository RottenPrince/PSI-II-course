using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBoxAPI.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class QuestionRuns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_SolveRunModels_RoomId",
                table: "SolveRunModels",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionModelSolveRunModel");

            migrationBuilder.DropTable(
                name: "SolveRunModels");
        }
    }
}
