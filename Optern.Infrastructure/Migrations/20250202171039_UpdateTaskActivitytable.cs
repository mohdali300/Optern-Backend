using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskActivitytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "TaskActivity",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskActivity_CreatorId",
                table: "TaskActivity",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskActivity_AspNetUsers_CreatorId",
                table: "TaskActivity",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskActivity_AspNetUsers_CreatorId",
                table: "TaskActivity");

            migrationBuilder.DropIndex(
                name: "IX_TaskActivity_CreatorId",
                table: "TaskActivity");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "TaskActivity");
        }
    }
}
