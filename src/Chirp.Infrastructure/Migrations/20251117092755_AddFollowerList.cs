using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowerList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFollowedByList",
                columns: table => new
                {
                    AId = table.Column<string>(type: "TEXT", nullable: false),
                    BId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollowedByList", x => new { x.AId, x.BId });
                    table.ForeignKey(
                        name: "FK_UserFollowedByList_AspNetUsers_AId",
                        column: x => x.AId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserFollowedByList_AspNetUsers_BId",
                        column: x => x.BId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowedByList_BId",
                table: "UserFollowedByList",
                column: "BId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFollowedByList");
        }
    }
}
