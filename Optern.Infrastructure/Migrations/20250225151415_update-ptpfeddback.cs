using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateptpfeddback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coding",
                table: "PTPFeedBacks");

            migrationBuilder.DropColumn(
                name: "Communication",
                table: "PTPFeedBacks");

            migrationBuilder.DropColumn(
                name: "Performance",
                table: "PTPFeedBacks");

            migrationBuilder.DropColumn(
                name: "ProblemSolving",
                table: "PTPFeedBacks");

            migrationBuilder.AddColumn<string>(
                name: "Ratings",
                table: "PTPFeedBacks",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ratings",
                table: "PTPFeedBacks");

            migrationBuilder.AddColumn<int>(
                name: "Coding",
                table: "PTPFeedBacks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Communication",
                table: "PTPFeedBacks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Performance",
                table: "PTPFeedBacks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProblemSolving",
                table: "PTPFeedBacks",
                type: "integer",
                nullable: true);
        }
    }
}
