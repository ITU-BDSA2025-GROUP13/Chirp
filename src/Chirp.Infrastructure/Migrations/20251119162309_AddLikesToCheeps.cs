using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLikesToCheeps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheepChirpUser",
                columns: table => new
                {
                    LikedCheepsCheepId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersWhoLikedId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheepChirpUser", x => new { x.LikedCheepsCheepId, x.UsersWhoLikedId });
                    table.ForeignKey(
                        name: "FK_CheepChirpUser_AspNetUsers_UsersWhoLikedId",
                        column: x => x.UsersWhoLikedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheepChirpUser_message_LikedCheepsCheepId",
                        column: x => x.LikedCheepsCheepId,
                        principalTable: "message",
                        principalColumn: "message_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheepChirpUser_UsersWhoLikedId",
                table: "CheepChirpUser",
                column: "UsersWhoLikedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheepChirpUser");
        }
    }
}
