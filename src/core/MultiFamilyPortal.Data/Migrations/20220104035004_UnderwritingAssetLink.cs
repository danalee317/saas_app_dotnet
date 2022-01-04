using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class UnderwritingAssetLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "UnderwritingPropertyProspects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnderwritingId",
                table: "AssetsUnderManagement",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "UnderwritingId",
                table: "AssetsUnderManagement");
        }
    }
}
