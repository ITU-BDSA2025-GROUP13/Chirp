using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplyToCheeps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentCheepCheepId",
                table: "message",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_message_ParentCheepCheepId",
                table: "message",
                column: "ParentCheepCheepId");

            migrationBuilder.AddForeignKey(
                name: "FK_message_message_ParentCheepCheepId",
                table: "message",
                column: "ParentCheepCheepId",
                principalTable: "message",
                principalColumn: "message_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_message_message_ParentCheepCheepId",
                table: "message");

            migrationBuilder.DropIndex(
                name: "IX_message_ParentCheepCheepId",
                table: "message");

            migrationBuilder.DropColumn(
                name: "ParentCheepCheepId",
                table: "message");
        }
    }
}
