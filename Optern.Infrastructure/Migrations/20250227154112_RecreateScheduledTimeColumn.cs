using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RecreateScheduledTimeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledTime",
                table: "PTPInterviews");

            
            migrationBuilder.AddColumn<int>(
                name: "ScheduledTime",
                table: "PTPInterviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);  
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledTime",
                table: "PTPInterviews");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ScheduledTime",
                table: "PTPInterviews",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0));
        }

    }
}
