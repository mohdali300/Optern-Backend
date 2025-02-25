using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePTPinterviewtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkSpaces_CreatedDate_Future",
                table: "WorkSpaces");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "PTPInterviews");

            migrationBuilder.RenameColumn(
                name: "IntervieweePerformance",
                table: "PTPFeedBacks",
                newName: "Improvement");

            migrationBuilder.AddColumn<string>(
                name: "Hints",
                table: "PTPQuestions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QusestionType",
                table: "PTPQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ScheduledTime",
                table: "PTPInterviews",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDate",
                table: "PTPInterviews",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SlotState",
                table: "PTPInterviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Coding",
                table: "PTPFeedBacks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Communication",
                table: "PTPFeedBacks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GivenByUserId",
                table: "PTPFeedBacks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Performance",
                table: "PTPFeedBacks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProblemSolving",
                table: "PTPFeedBacks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedByUserId",
                table: "PTPFeedBacks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

           

            migrationBuilder.CreateIndex(
                name: "IX_PTPFeedBacks_GivenByUserId_ReceivedByUserId_PTPInterviewId",
                table: "PTPFeedBacks",
                columns: new[] { "GivenByUserId", "ReceivedByUserId", "PTPInterviewId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PTPFeedBacks_ReceivedByUserId",
                table: "PTPFeedBacks",
                column: "ReceivedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PTPFeedBacks_PTPUsers_GivenByUserId",
                table: "PTPFeedBacks",
                column: "GivenByUserId",
                principalTable: "PTPUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PTPFeedBacks_PTPUsers_ReceivedByUserId",
                table: "PTPFeedBacks",
                column: "ReceivedByUserId",
                principalTable: "PTPUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PTPFeedBacks_PTPUsers_GivenByUserId",
                table: "PTPFeedBacks");

            migrationBuilder.DropForeignKey(
                name: "FK_PTPFeedBacks_PTPUsers_ReceivedByUserId",
                table: "PTPFeedBacks");

            migrationBuilder.DropIndex(
                name: "IX_PTPFeedBacks_GivenByUserId_ReceivedByUserId_PTPInterviewId",
                table: "PTPFeedBacks");

            migrationBuilder.DropIndex(
                name: "IX_PTPFeedBacks_ReceivedByUserId",
                table: "PTPFeedBacks");

            migrationBuilder.DropColumn(
                name: "Hints",
                table: "PTPQuestions");

            migrationBuilder.DropColumn(
                name: "QusestionType",
                table: "PTPQuestions");

            migrationBuilder.DropColumn(
                name: "ScheduledDate",
                table: "PTPInterviews");

            migrationBuilder.DropColumn(
                name: "SlotState",
                table: "PTPInterviews");

            migrationBuilder.DropColumn(
                name: "Coding",
                table: "PTPFeedBacks");

            migrationBuilder.DropColumn(
                name: "Communication",
                table: "PTPFeedBacks");

            migrationBuilder.DropColumn(
                name: "GivenByUserId",
                table: "PTPFeedBacks");

            migrationBuilder.DropColumn(
                name: "Performance",
                table: "PTPFeedBacks");

            migrationBuilder.DropColumn(
                name: "ProblemSolving",
                table: "PTPFeedBacks");

            migrationBuilder.DropColumn(
                name: "ReceivedByUserId",
                table: "PTPFeedBacks");

            migrationBuilder.RenameColumn(
                name: "Improvement",
                table: "PTPFeedBacks",
                newName: "IntervieweePerformance");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledTime",
                table: "PTPInterviews",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "PTPInterviews",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldDefaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkSpaces_CreatedDate_Future",
                table: "WorkSpaces",
                sql: "CreatedDate > NOW()");
        }
    }
}
