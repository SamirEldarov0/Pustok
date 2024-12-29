using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustok2.Migrations
{
    public partial class AuthorTableChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Authors");

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "Authors",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "Authors",
                type: "nvarchar(90)",
                maxLength: 90,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Img",
                table: "Authors");

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "Authors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(600)",
                oldMaxLength: 600,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Authors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
