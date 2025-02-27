using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatePTPQuestionInterview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PTPUserId",
                table: "PTPQuestionInterviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PTPQuestionInterviews_PTPUserId",
                table: "PTPQuestionInterviews",
                column: "PTPUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PTPQuestionInterviews_PTPUsers_PTPUserId",
                table: "PTPQuestionInterviews",
                column: "PTPUserId",
                principalTable: "PTPUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PTPQuestionInterviews_PTPUsers_PTPUserId",
                table: "PTPQuestionInterviews");

            migrationBuilder.DropIndex(
                name: "IX_PTPQuestionInterviews_PTPUserId",
                table: "PTPQuestionInterviews");

            migrationBuilder.DropColumn(
                name: "PTPUserId",
                table: "PTPQuestionInterviews");
        }
    }
}
