using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class removevfeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VFeedBack");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VFeedBack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VirtualInterviewId = table.Column<int>(type: "integer", nullable: false),
                    PerformanceScore = table.Column<int>(type: "integer", nullable: false, comment: "Performance score must be between 0 and 100 inclusive."),
                    Recommendations = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Strengths = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    VInterviewID = table.Column<int>(type: "integer", nullable: false),
                    Weaknesses = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VFeedBack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VFeedBack_VInterview_VInterviewID",
                        column: x => x.VInterviewID,
                        principalTable: "VInterview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VFeedBack_VInterview_VirtualInterviewId",
                        column: x => x.VirtualInterviewId,
                        principalTable: "VInterview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VFeedBack_VInterviewID",
                table: "VFeedBack",
                column: "VInterviewID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VFeedBack_VirtualInterviewId",
                table: "VFeedBack",
                column: "VirtualInterviewId");
        }
    }
}
