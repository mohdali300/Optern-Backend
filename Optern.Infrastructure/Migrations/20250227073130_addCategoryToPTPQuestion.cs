using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addCategoryToPTPQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "PTPQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "PTPQuestions");
        }
    }
}
