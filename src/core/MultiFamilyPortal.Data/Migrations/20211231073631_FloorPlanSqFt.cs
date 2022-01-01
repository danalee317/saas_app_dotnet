using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class FloorPlanSqFt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SquareFeet",
                table: "UnderwritingPropertyUnitModels",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SquareFeet",
                table: "UnderwritingPropertyUnitModels");
        }
    }
}
