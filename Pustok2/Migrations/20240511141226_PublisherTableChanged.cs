using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustok2.Migrations
{
    public partial class PublisherTableChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Pulishers_PublisherId",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pulishers",
                table: "Pulishers");

            migrationBuilder.RenameTable(
                name: "Pulishers",
                newName: "Publishers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Publishers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Publishers",
                table: "Publishers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Publishers",
                table: "Publishers");

            migrationBuilder.RenameTable(
                name: "Publishers",
                newName: "Pulishers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Pulishers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pulishers",
                table: "Pulishers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Pulishers_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Pulishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
