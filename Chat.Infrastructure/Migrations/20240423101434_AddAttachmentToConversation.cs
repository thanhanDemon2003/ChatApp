using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentToConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConversationId",
                table: "Attachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ConversationId",
                table: "Attachments",
                column: "ConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Conversations_ConversationId",
                table: "Attachments",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Conversations_ConversationId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_ConversationId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "Attachments");
        }
    }
}
