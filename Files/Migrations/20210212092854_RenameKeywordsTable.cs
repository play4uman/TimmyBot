using Microsoft.EntityFrameworkCore.Migrations;

namespace Files.Migrations
{
    public partial class RenameKeywordsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileMetadataKeyword_Keyword_KeywordId",
                table: "FileMetadataKeyword");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Keyword",
                table: "Keyword");

            migrationBuilder.RenameTable(
                name: "Keyword",
                newName: "Keywords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Keywords",
                table: "Keywords",
                column: "Word");

            migrationBuilder.AddForeignKey(
                name: "FK_FileMetadataKeyword_Keywords_KeywordId",
                table: "FileMetadataKeyword",
                column: "KeywordId",
                principalTable: "Keywords",
                principalColumn: "Word",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileMetadataKeyword_Keywords_KeywordId",
                table: "FileMetadataKeyword");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Keywords",
                table: "Keywords");

            migrationBuilder.RenameTable(
                name: "Keywords",
                newName: "Keyword");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Keyword",
                table: "Keyword",
                column: "Word");

            migrationBuilder.AddForeignKey(
                name: "FK_FileMetadataKeyword_Keyword_KeywordId",
                table: "FileMetadataKeyword",
                column: "KeywordId",
                principalTable: "Keyword",
                principalColumn: "Word",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
