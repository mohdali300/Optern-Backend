﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addattachmentdateUserTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Attachmentdate",
                table: "UserTasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachmentdate",
                table: "UserTasks");
        }
    }
}
