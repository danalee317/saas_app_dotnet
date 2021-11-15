using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class BucketList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BucketListId",
                table: "UnderwritingPropertyProspects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "NeighborhoodClass",
                table: "UnderwritingPropertyProspects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PropertyClass",
                table: "UnderwritingPropertyProspects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UnderwritingProspectPropertyBucketLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValuePlays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConstructionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UtilityNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompetitionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HowUnderwritingWasDetermined = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnderwritingProspectPropertyBucketLists");

            migrationBuilder.DropColumn(
                name: "BucketListId",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "NeighborhoodClass",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "PropertyClass",
                table: "UnderwritingPropertyProspects");
        }
    }
}
