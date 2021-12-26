using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class RenameDealAnalysis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnderwritingProspectPropertyBucketLists");

            migrationBuilder.CreateTable(
                name: "UnderwritingProspectPropertyDealAnalysis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValuePlays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConstructionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UtilityNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompetitionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HowUnderwritingWasDetermined = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarketPricePerUnit = table.Column<double>(type: "float", nullable: false),
                    MarketCapRate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnderwritingProspectPropertyDealAnalysis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnderwritingProspectPropertyDealAnalysis_UnderwritingPropertyProspects_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "UnderwritingPropertyProspects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnderwritingProspectPropertyDealAnalysis_PropertyId",
                table: "UnderwritingProspectPropertyDealAnalysis",
                column: "PropertyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnderwritingProspectPropertyDealAnalysis");

            migrationBuilder.CreateTable(
                name: "UnderwritingProspectPropertyBucketLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConstructionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HowUnderwritingWasDetermined = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarketCapRate = table.Column<double>(type: "float", nullable: false),
                    MarketPricePerUnit = table.Column<double>(type: "float", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UtilityNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValuePlays = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnderwritingProspectPropertyBucketLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnderwritingProspectPropertyBucketLists_UnderwritingPropertyProspects_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "UnderwritingPropertyProspects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnderwritingProspectPropertyBucketLists_PropertyId",
                table: "UnderwritingProspectPropertyBucketLists",
                column: "PropertyId",
                unique: true);
        }
    }
}
