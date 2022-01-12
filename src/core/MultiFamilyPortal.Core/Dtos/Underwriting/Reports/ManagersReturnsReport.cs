namespace MultiFamilyPortal.Dtos.Underwriting.Reports
{
    public class ManagersReturnsReport
    {
        public ManagersReturnsReport(UnderwritingAnalysis analysis)
        {
            EqualityOnSaleOfProperty = -6214973; // TODO: calculate
            ManagerEquity = (analysis.NOI - analysis.CapXTotal - analysis.AnnualDebtService) * analysis.OurEquityOfCF;
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
