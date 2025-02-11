using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editRoomChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Rooms_RoomId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_RoomId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "SentDate",
                table: "Messages",
                newName: "SentAt");

            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ChatId",
                table: "Rooms",
                column: "ChatId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Chats_ChatId",
                table: "Rooms",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Chats_ChatId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_ChatId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Messages",
                newName: "SentDate");

            migrationBuilder.AddColumn<string>(
                name: "RoomId",
                table: "Chats",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_RoomId",
                table: "Chats",
                column: "RoomId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Rooms_RoomId",
                table: "Chats",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
