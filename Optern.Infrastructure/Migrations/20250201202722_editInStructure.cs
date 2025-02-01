using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Optern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editInStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomTracks_SubTracks_SubTrackId",
                table: "RoomTracks");

            migrationBuilder.DropTable(
                name: "SubTracks");

            migrationBuilder.RenameColumn(
                name: "SubTrackId",
                table: "RoomTracks",
                newName: "TrackId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomTracks_SubTrackId",
                table: "RoomTracks",
                newName: "IX_RoomTracks_TrackId");

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PositionId = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomPositions_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomPositions_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PositionId",
                table: "AspNetUsers",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPositions_PositionId",
                table: "RoomPositions",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPositions_RoomId",
                table: "RoomPositions",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Positions_PositionId",
                table: "AspNetUsers",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomTracks_Tracks_TrackId",
                table: "RoomTracks",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Positions_PositionId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomTracks_Tracks_TrackId",
                table: "RoomTracks");

            migrationBuilder.DropTable(
                name: "RoomPositions");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PositionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "TrackId",
                table: "RoomTracks",
                newName: "SubTrackId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomTracks_TrackId",
                table: "RoomTracks",
                newName: "IX_RoomTracks_SubTrackId");

            migrationBuilder.CreateTable(
                name: "SubTracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrackId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubTracks_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubTracks_TrackId",
                table: "SubTracks",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomTracks_SubTracks_SubTrackId",
                table: "RoomTracks",
                column: "SubTrackId",
                principalTable: "SubTracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
