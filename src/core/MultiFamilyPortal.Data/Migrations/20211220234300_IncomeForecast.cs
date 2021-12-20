using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class IncomeForecast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnderwritingProspectPropertyIncomeForecasts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProspectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IncreaseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerUnitIncrease = table.Column<double>(type: "float", nullable: false),
                    UnitsAppliedTo = table.Column<int>(type: "int", nullable: false),
                    FixedIncreaseOnRemainingUnits = table.Column<double>(type: "float", nullable: false),
                    Vacancy = table.Column<double>(type: "float", nullable: false),
                    OtherLossesPercent = table.Column<double>(type: "float", nullable: false),
                    UtilityIncreases = table.Column<double>(type: "float", nullable: false),
                    OtherIncomePercent = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnderwritingProspectPropertyIncomeForecasts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnderwritingProspectPropertyIncomeForecasts_UnderwritingPropertyProspects_ProspectId",
                        column: x => x.ProspectId,
                        principalTable: "UnderwritingPropertyProspects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnderwritingProspectPropertyIncomeForecasts_ProspectId",
                table: "UnderwritingProspectPropertyIncomeForecasts",
                column: "ProspectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnderwritingProspectPropertyIncomeForecasts");
        }
    }
}
