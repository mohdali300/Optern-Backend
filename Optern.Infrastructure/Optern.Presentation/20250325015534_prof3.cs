using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class prof3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerformanceScore",
                table: "VFeedBack");

            migrationBuilder.DropColumn(
                name: "Strengths",
                table: "VFeedBack");

            migrationBuilder.RenameColumn(
                name: "Weaknesses",
                table: "VFeedBack",
                newName: "Intro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Intro",
                table: "VFeedBack",
                newName: "Weaknesses");

            migrationBuilder.AddColumn<int>(
                name: "PerformanceScore",
                table: "VFeedBack",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Performance score must be between 0 and 100 inclusive.");

            migrationBuilder.AddColumn<string>(
                name: "Strengths",
                table: "VFeedBack",
                type: "text",
                nullable: true);
        }
    }
}
