using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class addVQuestionInterviewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PTPQuestionInterviews_VInterview_VInterviewId",
                table: "PTPQuestionInterviews");

            migrationBuilder.DropIndex(
                name: "IX_PTPQuestionInterviews_VInterviewId",
                table: "PTPQuestionInterviews");

            migrationBuilder.DropColumn(
                name: "VInterviewId",
                table: "PTPQuestionInterviews");

            migrationBuilder.AlterColumn<int>(
                name: "PTPInterviewId",
                table: "PTPQuestionInterviews",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "VQuestionInterviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PTPQuestionId = table.Column<int>(type: "integer", nullable: false),
                    VInterviewId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VQuestionInterviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VQuestionInterviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VQuestionInterviews_PTPQuestions_PTPQuestionId",
                        column: x => x.PTPQuestionId,
                        principalTable: "PTPQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VQuestionInterviews_VInterview_VInterviewId",
                        column: x => x.VInterviewId,
                        principalTable: "VInterview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VQuestionInterview_PTPQuestionId",
                table: "VQuestionInterviews",
                column: "PTPQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_VQuestionInterviews_UserId",
                table: "VQuestionInterviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VQuestionInterviews_VInterviewId",
                table: "VQuestionInterviews",
                column: "VInterviewId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VQuestionInterviews");

            migrationBuilder.AlterColumn<int>(
                name: "PTPInterviewId",
                table: "PTPQuestionInterviews",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

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
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
