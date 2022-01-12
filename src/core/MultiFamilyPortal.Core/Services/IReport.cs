using MultiFamilyPortal.Dtos.Underwriting.Reports;
using Telerik.Windows.Documents.Fixed.Model;

namespace MultiFamilyPortal.Services;

public interface IReport
{
   RadFixedDocument OverallProjections();
   RadFixedDocument CashFlow();
   RadFixedDocument ManagersReturns(ManagersReturnsReport mmr);
   RadFixedDocument ThreeTier();
   RadFixedDocument CulmativeInvestment();
   RadFixedDocument AOneandAtwo();
   RadFixedDocument ThousandInvestmentProjects();
   RadFixedDocument NetPresentValue();
   RadFixedDocument LeveragedRateOfReturns();
   byte[] ExportToPDF(RadFixedDocument doc);
}