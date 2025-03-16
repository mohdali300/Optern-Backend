using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class Addvfeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VFeedBack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerformanceScore = table.Column<int>(type: "integer", nullable: false, comment: "Performance score must be between 0 and 100 inclusive."),
                    Strengths = table.Column<string>(type: "text", nullable: true),
                    Weaknesses = table.Column<string>(type: "text", nullable: true),
                    Recommendations = table.Column<string>(type: "text", nullable: true),
                    VirtualInterviewId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VFeedBack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VFeedBack_VInterview_VirtualInterviewId",
                        column: x => x.VirtualInterviewId,
                        principalTable: "VInterview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VFeedBack_VirtualInterviewId",
                table: "VFeedBack",
                column: "VirtualInterviewId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VFeedBack");
        }
    }
}
