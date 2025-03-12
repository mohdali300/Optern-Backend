using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Optern.Infrastructure.Optern.Presentation
{
    /// <inheritdoc />
    public partial class updateVinterviewconfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VInterview_VFeedBack_Id",
                table: "VInterview");

            migrationBuilder.DropCheckConstraint(
                name: "CK_VFeedBack_PerformanceScore",
                table: "VFeedBack");

            migrationBuilder.AlterColumn<string>(
                name: "SpeechAnalysisResult",
                table: "VInterview",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "VInterview",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_VFeedBack_VInterviewID",
                table: "VFeedBack",
                column: "VInterviewID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VFeedBack_VInterview_VInterviewID",
                table: "VFeedBack",
                column: "VInterviewID",
                principalTable: "VInterview",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VFeedBack_VInterview_VInterviewID",
                table: "VFeedBack");

            migrationBuilder.DropIndex(
                name: "IX_VFeedBack_VInterviewID",
                table: "VFeedBack");

            migrationBuilder.AlterColumn<string>(
                name: "SpeechAnalysisResult",
                table: "VInterview",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "VInterview",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddCheckConstraint(
                name: "CK_VFeedBack_PerformanceScore",
                table: "VFeedBack",
                sql: "\"PerformanceScore\" BETWEEN 0 AND 100");

            migrationBuilder.AddForeignKey(
                name: "FK_VInterview_VFeedBack_Id",
                table: "VInterview",
                column: "Id",
                principalTable: "VFeedBack",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
