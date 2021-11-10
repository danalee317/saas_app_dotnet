using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class AssetStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Highlighted",
                table: "AssetsUnderManagement",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "SalesPrice",
                table: "AssetsUnderManagement",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AssetsUnderManagement",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Highlighted",
                table: "AssetsUnderManagement");

            migrationBuilder.DropColumn(
                name: "SalesPrice",
                table: "AssetsUnderManagement");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AssetsUnderManagement");
        }
    }
}
