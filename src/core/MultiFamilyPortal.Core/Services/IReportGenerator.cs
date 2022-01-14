using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.Services;

public interface IReportGenerator
{
    Task<ReportResponse> OverallProjections(Guid propertyId);
    Task<ReportResponse> CashFlow(Guid propertyId);
    Task<ReportResponse> ManagersReturns(Guid propertyId);
    Task<ReportResponse> ThreeTier(Guid propertyId);
    Task<ReportResponse> CulmativeInvestment(Guid propertyId);
    Task<ReportResponse> AOneandAtwo(Guid propertyId);
    Task<ReportResponse> ThousandInvestmentProjects(Guid propertyId);
    Task<ReportResponse> NetPresentValue(Guid propertyId);
    Task<ReportResponse> LeveragedRateOfReturns(Guid propertyId);
}