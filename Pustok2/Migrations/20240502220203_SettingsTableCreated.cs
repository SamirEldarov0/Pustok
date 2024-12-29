using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustok2.Migrations
{
    public partial class SettingsTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupportPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HeaderLogo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FooterLogo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PromotionTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PromotionSubtitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PromotionBtnText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PromotionRedirectURL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PromotionBgImage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
