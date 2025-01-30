using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editBookMarkedTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookMarkedTasks_Posts_PostId",
                table: "BookMarkedTasks");

            migrationBuilder.DropIndex(
                name: "IX_BookMarkedTasks_PostId",
                table: "BookMarkedTasks");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "BookMarkedTasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "BookMarkedTasks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookMarkedTasks_PostId",
                table: "BookMarkedTasks",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookMarkedTasks_Posts_PostId",
                table: "BookMarkedTasks",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
