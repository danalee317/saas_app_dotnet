using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class DisplayUnit
    {
        public DisplayUnit(UnderwritingAnalysisUnit unit, UnderwritingAnalysisModel floorPlan)
        {
            Unit = unit;
            FloorPlan = floorPlan;
        }

        public UnderwritingAnalysisModel FloorPlan { get; set; }

        public UnderwritingAnalysisUnit Unit { get; }

        public string FloorPlanName => FloorPlan.Name;

        public Guid Id
        {
            get => Unit.Id;
            set => Unit.Id = value;
        }

        public string UnitName
        {
            get => Unit.Unit;
            set => Unit.Unit = value;
        }

        public string Renter
        {
            get => Unit.Renter;
            set => Unit.Renter = value;
        }

        public DateTime? LeaseStart
        {
            get => Unit.LeaseStart;
            set => Unit.LeaseStart = value;
        }

        public DateTime? LeaseEnd
        {
            get => Unit.LeaseEnd;
            set => Unit.LeaseEnd = value;
        }

        public bool AtWill
        {
            get => Unit.AtWill;
            set => Unit.AtWill = value;
        }

        public double Rent
        {
            get => Unit.Rent;
            set => Unit.Rent = value;
        }

        public double DepositOnHand
        {
            get => Unit.DepositOnHand;
            set => Unit.DepositOnHand = value;
        }

        public double Balance
        {
            get => Unit.Balance;
            set => Unit.Balance = value;
        }

        public List<UnderwritingAnalysisUnitLedgerItem> Ledger
        {
            get => Unit.Ledger;
            set => Unit.Ledger = value;
        }
    }
}
