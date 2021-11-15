using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class MarketInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MarketCapRate",
                table: "UnderwritingProspectPropertyBucketLists",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MarketPricePerUnit",
                table: "UnderwritingProspectPropertyBucketLists",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketCapRate",
                table: "UnderwritingProspectPropertyBucketLists");

            migrationBuilder.DropColumn(
                name: "MarketPricePerUnit",
                table: "UnderwritingProspectPropertyBucketLists");
        }
    }
}
