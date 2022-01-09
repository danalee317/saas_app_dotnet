using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class InvestmentTier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ReversionCapRate",
                table: "UnderwritingPropertyProspects",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "UnderwritingTiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Group = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Investment = table.Column<double>(type: "float", nullable: false),
                    PreferredRoR = table.Column<double>(type: "float", nullable: false),
                    RoROnSale = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnderwritingTiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnderwritingTiers_UnderwritingPropertyProspects_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "UnderwritingPropertyProspects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnderwritingTiers_PropertyId",
                table: "UnderwritingTiers",
                column: "PropertyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnderwritingTiers");

            migrationBuilder.DropColumn(
                name: "ReversionCapRate",
                table: "UnderwritingPropertyProspects");
        }
    }
}
