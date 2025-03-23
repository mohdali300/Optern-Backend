using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class UpdateNotificationUserNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Rooms_RoomId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Notifications");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "UserNotifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "UserNotifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Rooms_RoomId",
                table: "Notifications",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Rooms_RoomId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "UserNotifications");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Rooms_RoomId",
                table: "Notifications",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
