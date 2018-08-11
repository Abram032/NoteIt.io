using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NoteIt.Infrastructure.Data.Migrations
{
    public partial class GuidShared : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Notes",
                nullable: false,
                oldClrType: typeof(long))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                table: "Notes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Contents",
                nullable: false,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShared",
                table: "Notes");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Notes",
                nullable: false,
                oldClrType: typeof(Guid))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Contents",
                nullable: false,
                oldClrType: typeof(Guid));
        }
    }
}
