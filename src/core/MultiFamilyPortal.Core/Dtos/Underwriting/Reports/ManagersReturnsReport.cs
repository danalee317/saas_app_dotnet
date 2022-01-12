namespace MultiFamilyPortal.Dtos.Underwriting.Reports
{
    public class ManagersReturnsReport
    {
        public ManagersReturnsReport(UnderwritingAnalysis analysis)
        {
            /* Do the math here  */
            /*  AquisitionFee  = analysis.AquisitionFee;
             EqualityOnSaleOfProperty= 0.0;
             HoldYears = analysis.HoldYears;
             ManagerEquity = analysis.OurEquityOfCF;
             */
            // Test
            CashFlowPercentage = 0.25;
            AcquisitionFee = 50000;
            ManagerEquity = 11852679;
            EqualityOnSaleOfProperty = -6214973;
            HoldYears = 10;
        }

        public double AcquisitionFee { get; }
        public double ManagerEquity { get; }
        public double CashFlowPercentage { get; }
        public double EqualityOnSaleOfProperty { get; }
        public int HoldYears { get; }
    }
}
