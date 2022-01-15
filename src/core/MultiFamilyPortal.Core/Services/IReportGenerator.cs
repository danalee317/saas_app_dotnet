using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.Services;

public interface IReportGenerator
{
    Task<ReportResponse> OverallProjections(Guid propertyId);
    Task<ReportResponse> CashFlow(Guid propertyId);
    Task<ReportResponse> ManagersReturns(Guid propertyId);
    Task<ReportResponse> TieredInvestmentGroup(Guid propertyId, string groupName);
    Task<ReportResponse> CulmativeInvestment(Guid propertyId);
    Task<ReportResponse> IncomeForecast(Guid propertyId);
    Task<ReportResponse> CapitalExpenses(Guid propertyId);
    Task<ReportResponse> ThousandInvestmentProjects(Guid propertyId);
    Task<ReportResponse> NetPresentValue(Guid propertyId);
    Task<ReportResponse> LeveragedRateOfReturns(Guid propertyId);
}
