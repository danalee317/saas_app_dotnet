using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class UpgradableFloorPlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CurrentRent",
                table: "UnderwritingPropertyUnitModels",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TotalUnits",
                table: "UnderwritingPropertyUnitModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Upgraded",
                table: "UnderwritingPropertyUnitModels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentRent",
                table: "UnderwritingPropertyUnitModels");

            migrationBuilder.DropColumn(
                name: "TotalUnits",
                table: "UnderwritingPropertyUnitModels");

            migrationBuilder.DropColumn(
                name: "Upgraded",
                table: "UnderwritingPropertyUnitModels");
        }
    }
}
