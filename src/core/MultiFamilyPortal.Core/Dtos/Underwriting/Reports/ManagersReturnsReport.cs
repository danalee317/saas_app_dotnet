namespace MultiFamilyPortal.Dtos.Underwriting.Reports
{
    public class ManagersReturnsReport
    {
        public ManagersReturnsReport(UnderwritingAnalysis analysis)
        {
            EqualityOnSaleOfProperty = analysis.Projections.Select(x => x.Equity).Sum();
            ManagerEquity = (analysis.NOI - analysis.CapXTotal - analysis.Projections.Select(x => x.RemainingDebt).Sum()) * analysis.OurEquityOfCF;
            CashFlowPercentage = analysis.OurEquityOfCF;
            HoldYears = analysis.HoldYears;
            AcquisitionFee = analysis.AquisitionFee;
        }

        public double AcquisitionFee { get; }
        public double ManagerEquity { get; }
        public double CashFlowPercentage { get; }
        public double EqualityOnSaleOfProperty { get; }
        public int HoldYears { get; }
    }
}
