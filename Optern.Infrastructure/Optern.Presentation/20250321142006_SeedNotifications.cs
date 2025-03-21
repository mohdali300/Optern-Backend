using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class SeedNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
           table: "Notifications",
           columns: new[] { "Id", "Title", "Message", "Url", "CreatedTime" },
           values: new object[,]
           {
                { 1, "Welcome to Optern! 🎉", "We're thrilled to have you join our community! To make the most out of your experience and enjoy all the amazing features, please complete your profile. Click here to get started now!", "/profile", DateTime.UtcNow },
                { 2, "New Task Added 📝", "A task has been added by you!", "/tasks", DateTime.UtcNow },
                { 3, "Task Assigned 📌", "A new task has been assigned to you!", "/tasks/assigned", DateTime.UtcNow },
                { 4, "Task Removed ❌", "A task has been removed from your list.", "/tasks/removed", DateTime.UtcNow },
                { 5, "Admin Role Assigned 🔥", "You have been assigned as an Admin!", "/admin/dashboard", DateTime.UtcNow },
                { 6, "Task Not Assigned ⚠️", "You have been removed from a task.", "/tasks/pending", DateTime.UtcNow },
                { 7, "You Got a Message 💬", "You have received a new message.", "/messages", DateTime.UtcNow },
                { 8, "Peer-to-Peer Interview 📅", "You have a Peer-to-Peer interview in 30 minutes. Get ready!", "/interviews/upcoming", DateTime.UtcNow },
                { 9, "Peer-to-Peer Interview Reminder ⏳", "You have a Peer-to-Peer interview in 5 minutes. Get ready!", "/interviews/upcoming", DateTime.UtcNow }
           }
       );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
           table: "Notifications",
           keyColumn: "Id",
           keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
       );
        }
    }
}
