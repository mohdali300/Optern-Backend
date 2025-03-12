using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class UpdateinVInterviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VInterviewQuestions");

            migrationBuilder.DropTable(
                name: "VQuestions");

            migrationBuilder.DropColumn(
                name: "ScheduledTime",
                table: "VInterview");

            migrationBuilder.AddColumn<string>(
                name: "QusestionType",
                table: "VInterview",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VInterviewId",
                table: "PTPQuestionInterviews",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PTPQuestionInterviews_VInterviewId",
                table: "PTPQuestionInterviews",
                column: "VInterviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_PTPQuestionInterviews_VInterview_VInterviewId",
                table: "PTPQuestionInterviews",
                column: "VInterviewId",
                principalTable: "VInterview",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PTPQuestionInterviews_VInterview_VInterviewId",
                table: "PTPQuestionInterviews");

            migrationBuilder.DropIndex(
                name: "IX_PTPQuestionInterviews_VInterviewId",
                table: "PTPQuestionInterviews");

            migrationBuilder.DropColumn(
                name: "QusestionType",
                table: "VInterview");

            migrationBuilder.DropColumn(
                name: "VInterviewId",
                table: "PTPQuestionInterviews");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledTime",
                table: "VInterview",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "ScheduledTime must be in the future and is required.");

            migrationBuilder.CreateTable(
                name: "VQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Answer = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false, comment: "Content of the question with a maximum of 500 characters.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VInterviewQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VInterviewID = table.Column<int>(type: "integer", nullable: false),
                    VQuestionID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VInterviewQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VInterviewQuestions_VInterview_VInterviewID",
                        column: x => x.VInterviewID,
                        principalTable: "VInterview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VInterviewQuestions_VQuestions_VQuestionID",
                        column: x => x.VQuestionID,
                        principalTable: "VQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VInterviewQuestions_VInterviewID",
                table: "VInterviewQuestions",
                column: "VInterviewID");

            migrationBuilder.CreateIndex(
                name: "IX_VInterviewQuestions_VQuestionID",
                table: "VInterviewQuestions",
                column: "VQuestionID");
        }
    }
}
