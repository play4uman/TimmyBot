using Microsoft.EntityFrameworkCore.Migrations;

namespace Files.Migrations
{
    public partial class AddParagraphDelimiter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreferredParagraphDelimiter",
                table: "FileMetadata",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredParagraphDelimiter",
                table: "FileMetadata");
        }
    }
}
