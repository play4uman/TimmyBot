using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Files.Migrations
{
    public partial class AddKeywordFileRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Keyword",
                columns: table => new
                {
                    Word = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keyword", x => x.Word);
                });

            migrationBuilder.CreateTable(
                name: "FileMetadataKeyword",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeywordId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Times = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMetadataKeyword", x => new { x.FileId, x.KeywordId });
                    table.ForeignKey(
                        name: "FK_FileMetadataKeyword_FileMetadata_FileId",
                        column: x => x.FileId,
                        principalTable: "FileMetadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileMetadataKeyword_Keyword_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keyword",
                        principalColumn: "Word",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadataKeyword_KeywordId",
                table: "FileMetadataKeyword",
                column: "KeywordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileMetadataKeyword");

            migrationBuilder.DropTable(
                name: "Keyword");
        }
    }
}
