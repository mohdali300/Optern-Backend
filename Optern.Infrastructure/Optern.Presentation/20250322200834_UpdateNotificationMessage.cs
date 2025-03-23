using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class UpdateNotificationMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 11,
                column: "Message",
                value: "You have been removed from the room. Click to view the details."
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
               table: "Notifications",
               keyColumn: "Id",
               keyValue: 11,
               column: "Message",
               value: "Request to join the room has been sent. Click the link to view the Details."
           );
        }
    }
}
