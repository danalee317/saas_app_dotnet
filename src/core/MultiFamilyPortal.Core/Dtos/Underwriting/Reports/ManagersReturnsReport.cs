namespace MultiFamilyPortal.Dtos.Underwriting.Reports
{
    public class ManagersReturnsReport
    {
        public ManagersReturnsReport(UnderwritingAnalysis analysis)
        {
            ManagerEquity = analysis.Projections.OrderBy(x => x.Year).FirstOrDefault().Equity;
            EqualityOnSaleOfProperty = analysis.Projections.OrderBy(x => x.Year).LastOrDefault().Equity;
            CashFlow = analysis.Projections.OrderBy(x => x.Year).Select(x => x.TotalCashFlow).ToList();
            CashFlowPercentage = analysis.OurEquityOfCF;
            HoldYears = analysis.HoldYears;
            AcquisitionFee = analysis.AquisitionFee;
        }

        public double AcquisitionFee { get; }
        public double ManagerEquity { get; }
        public IEnumerable<double> CashFlow { get; }
        public double CashFlowPercentage { get; }
        public double EqualityOnSaleOfProperty { get; }
        public int HoldYears { get; }
    }
}
