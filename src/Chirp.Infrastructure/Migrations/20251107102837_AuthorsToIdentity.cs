using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuthorsToIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_message_user_author_id",
                table: "message");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.AddForeignKey(
                name: "FK_message_AspNetUsers_author_id",
                table: "message",
                column: "author_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_message_AspNetUsers_author_id",
                table: "message");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    username = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_message_user_author_id",
                table: "message",
                column: "author_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
