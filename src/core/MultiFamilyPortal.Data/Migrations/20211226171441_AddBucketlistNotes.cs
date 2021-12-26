using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class AddBucketlistNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConcessionsNonPaymentNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractServicesNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneralAdminNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrossPotentialRentNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrossScheduledRentNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsuranceNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LossToLeaseNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagementNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarketingNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherIncomeNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayrollNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhysicalVacancyNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepairsMaintenanceNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxesNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UtilityNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UtilityReimbursementNotes",
                table: "UnderwritingPropertyProspects",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcessionsNonPaymentNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "ContractServicesNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "GeneralAdminNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "GrossPotentialRentNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "GrossScheduledRentNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "InsuranceNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "LossToLeaseNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "ManagementNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "MarketingNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "OtherIncomeNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "PayrollNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "PhysicalVacancyNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "RepairsMaintenanceNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "TaxesNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "UtilityNotes",
                table: "UnderwritingPropertyProspects");

            migrationBuilder.DropColumn(
                name: "UtilityReimbursementNotes",
                table: "UnderwritingPropertyProspects");
        }
    }
}
