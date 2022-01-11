using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class LendingNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LendingNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LendingNotes",
                table: "UnderwritingPropertyProspects");
        }
    }
}
