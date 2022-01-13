using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.Services;

public interface IReport
{
   ReportResponse OverallProjections();
   ReportResponse CashFlow();
   Task<ReportResponse> ManagersReturns(Guid propertyId);
   ReportResponse ThreeTier();
   ReportResponse CulmativeInvestment();
   ReportResponse AOneandAtwo();
   ReportResponse ThousandInvestmentProjects();
   ReportResponse NetPresentValue();
   ReportResponse LeveragedRateOfReturns();
}