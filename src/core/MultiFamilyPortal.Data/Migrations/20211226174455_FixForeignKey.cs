using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class FixForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BucketListId",
                table: "UnderwritingPropertyProspects",
                newName: "DealAnalysisId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DealAnalysisId",
                table: "UnderwritingPropertyProspects",
                newName: "BucketListId");
        }
    }
}
