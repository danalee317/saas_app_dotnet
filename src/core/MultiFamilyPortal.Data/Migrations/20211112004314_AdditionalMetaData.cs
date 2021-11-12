using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class AdditionalMetaData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnderwriterEmail",
                table: "UnderwritingProspectFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvestorState",
                table: "AssetsUnderManagement",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ActivityLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnderwriterEmail",
                table: "UnderwritingProspectFiles");

            migrationBuilder.DropColumn(
                name: "InvestorState",
                table: "AssetsUnderManagement");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ActivityLogs");
        }
    }
}
