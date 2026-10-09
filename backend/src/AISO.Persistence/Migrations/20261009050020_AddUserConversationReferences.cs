using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserConversationReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_conversation_references",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamsUserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ConversationReferenceJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_conversation_references", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_conversation_references_TeamsUserId",
                table: "user_conversation_references",
                column: "TeamsUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_conversation_references");
        }
    }
}
