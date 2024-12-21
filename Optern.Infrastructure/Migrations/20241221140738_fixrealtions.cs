using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixrealtions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkSpace_CreatedDate",
                table: "WorkSpaces");

            migrationBuilder.DropIndex(
                name: "IX_VInterview_ScheduledTime",
                table: "VInterview");

            migrationBuilder.DropIndex(
                name: "IX_Task_DueDate",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Task_EndDate",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Task_StartDate",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Sprint_StartDate",
                table: "Sprints");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_Capacity",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_CreatedAt",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Reacts_ReactType",
                table: "Reacts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CreatedDate",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Experiences_UserId_Company_JobTitle_StartDate",
                table: "Experiences");

            migrationBuilder.DropIndex(
                name: "IX_Education_University",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentDate",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_CommentReact_ReactType",
                table: "CommentReacts");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_SprintId",
                table: "Tasks",
                newName: "IX_Task_SprintId");

            migrationBuilder.AlterColumn<string>(
                name: "RoomId",
                table: "Notifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "SavedAt",
                table: "FavoritePosts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "RoomId",
                table: "Chats",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_UserId",
                table: "Experiences",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Experience_UserId",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "SavedAt",
                table: "FavoritePosts");

            migrationBuilder.RenameIndex(
                name: "IX_Task_SprintId",
                table: "Tasks",
                newName: "IX_Tasks_SprintId");

            migrationBuilder.AlterColumn<string>(
                name: "RoomId",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RoomId",
                table: "Chats",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkSpace_CreatedDate",
                table: "WorkSpaces",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_VInterview_ScheduledTime",
                table: "VInterview",
                column: "ScheduledTime");

            migrationBuilder.CreateIndex(
                name: "IX_Task_DueDate",
                table: "Tasks",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Task_EndDate",
                table: "Tasks",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Task_StartDate",
                table: "Tasks",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Sprint_StartDate",
                table: "Sprints",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Capacity",
                table: "Rooms",
                column: "Capacity");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CreatedAt",
                table: "Rooms",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Reacts_ReactType",
                table: "Reacts",
                column: "ReactType");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedDate",
                table: "Posts",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_UserId_Company_JobTitle_StartDate",
                table: "Experiences",
                columns: new[] { "UserId", "Company", "JobTitle", "StartDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Education_University",
                table: "Educations",
                column: "University");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentDate",
                table: "Comments",
                column: "CommentDate");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReact_ReactType",
                table: "CommentReacts",
                column: "ReactType");
        }
    }
}
