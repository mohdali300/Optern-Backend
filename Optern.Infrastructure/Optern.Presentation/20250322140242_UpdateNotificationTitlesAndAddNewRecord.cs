using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class UpdateNotificationTitlesAndAddNewRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
               table: "Notifications",
               keyColumn: "Id",
               keyValue: 2,
               column: "Title",
               value: "Tasks"
           );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 3,
                column: "Title",
                value: "Tasks"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 4,
                column: "Title",
                value: "Tasks"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 6,
                column: "Title",
                value: "Tasks"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 7,
                column: "Title",
                value: "Messages"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 8,
                column: "Title",
                value: "Interviews"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 9,
                column: "Title",
                value: "Interviews"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 5,
                column: "Title",
                value: "Admins"
            );

            // Inserting new notification records
            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "Title", "Message", "Url", "CreatedTime" },
                values: new object[,]
                {
                    { 10, "Admins", "You have been removed from being an admin.", "/admin/dashboard", DateTime.UtcNow },
                    { 11, "Rooms", "Request to join the room has been sent. Click the link to view the Details.", "/rooms/requests", DateTime.UtcNow },
                    { 12, "Rooms", "Your request to join the room has been accepted. Click the link to view the room.", "/rooms/joined", DateTime.UtcNow },
                    { 13, "Rooms", "Your request to join the room has been rejected. Click the link to view the Details.", "/rooms/rejected", DateTime.UtcNow }
                }
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
               table: "Notifications",
               keyColumn: "Id",
               keyValue: 2,
               column: "Title",
               value: "New Task Added 📝"
           );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 3,
                column: "Title",
                value: "Task Assigned 📌"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 4,
                column: "Title",
                value: "Task Removed ❌"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 6,
                column: "Title",
                value: "Task Not Assigned ⚠️"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 7,
                column: "Title",
                value: "You Got a Message 💬"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 8,
                column: "Title",
                value: "Peer-to-Peer Interview 📅"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 9,
                column: "Title",
                value: "Peer-to-Peer Interview Reminder ⏳"
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 5,
                column: "Title",
                value: "Admin Role Assigned 🔥"
            );

            // Removing new notifications
            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValues: new object[] { 10, 11, 12, 13 }
            );
        }
    }
}
