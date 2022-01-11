using MultiFamilyPortal.Dtos.Underwriting.Reports;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace MultiFamilyPortal.Services;

public interface IReport
{
    Workbook OverallProjections();
    Workbook CashFlow();
    Workbook ManagersReturns(ManagersReturnsReport mmr);
    Workbook ThreeTier();
    Workbook CulmativeInvestment();
    Workbook AOneandAtwo();
    Workbook ThousandInvestmentProjects();
    Workbook NetPresentValue();
    Workbook LeveragedRateOfReturns();
    bool ExportToPDF(Workbook workbook, string name, string path);
    bool ExportToSpreadsheet(Workbook workbook, string name, string folder);
}