using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstraintFromExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Experience_UserId",
                table: "Experiences");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_UserId",
                table: "Experiences",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Experience_UserId",
                table: "Experiences");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_UserId",
                table: "Experiences",
                column: "UserId",
                unique: true);
        }
    }
}
