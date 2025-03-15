using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class addVinterviewidinquestionconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PTPQuestionInterviews_VInterview_VInterviewId",
                table: "PTPQuestionInterviews");

            migrationBuilder.AlterColumn<int>(
                name: "PTPInterviewId",
                table: "PTPQuestionInterviews",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_PTPQuestionInterviews_VInterview_VInterviewId",
                table: "PTPQuestionInterviews",
                column: "VInterviewId",
                principalTable: "VInterview",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PTPQuestionInterviews_VInterview_VInterviewId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_PTPQuestionInterviews_VInterview_VInterviewId",
                table: "PTPQuestionInterviews",
                column: "VInterviewId",
                principalTable: "VInterview",
                principalColumn: "Id");
        }
    }
}
