using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editBookMarksRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookMarkedTasks_UserRooms_UserRoomId",
                table: "BookMarkedTasks");

            migrationBuilder.DropIndex(
                name: "IX_BookMarkedTasks_UserRoomId",
                table: "BookMarkedTasks");

            migrationBuilder.DropColumn(
                name: "UserRoomId",
                table: "BookMarkedTasks");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BookMarkedTasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BookMarkedTasks_UserId",
                table: "BookMarkedTasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookMarkedTasks_AspNetUsers_UserId",
                table: "BookMarkedTasks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookMarkedTasks_AspNetUsers_UserId",
                table: "BookMarkedTasks");

            migrationBuilder.DropIndex(
                name: "IX_BookMarkedTasks_UserId",
                table: "BookMarkedTasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BookMarkedTasks");

            migrationBuilder.AddColumn<int>(
                name: "UserRoomId",
                table: "BookMarkedTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookMarkedTasks_UserRoomId",
                table: "BookMarkedTasks",
                column: "UserRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookMarkedTasks_UserRooms_UserRoomId",
                table: "BookMarkedTasks",
                column: "UserRoomId",
                principalTable: "UserRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
