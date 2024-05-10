﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addLatestMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LatestMessage",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatestMessage",
                table: "Conversations");
        }
    }
}
