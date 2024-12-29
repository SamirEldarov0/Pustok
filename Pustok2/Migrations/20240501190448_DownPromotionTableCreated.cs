using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustok2.Migrations
{
    public partial class DownPromotionTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DownPromotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Subtitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BtnText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RedirectURL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DownPromotions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DownPromotions");
        }
    }
}
