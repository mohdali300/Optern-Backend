using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameforChatId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Chats_ChatID",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "ChatID",
                table: "Rooms",
                newName: "ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_ChatID",
                table: "Rooms",
                newName: "IX_Rooms_ChatId");

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

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "Rooms",
                newName: "ChatID");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_ChatId",
                table: "Rooms",
                newName: "IX_Rooms_ChatID");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Chats_ChatID",
                table: "Rooms",
                column: "ChatID",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
