using Microsoft.EntityFrameworkCore.Migrations;

namespace NoteIt.Infrastructure.Data.Migrations
{
    public partial class AddedUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Notes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Notes");
        }
    }
}
