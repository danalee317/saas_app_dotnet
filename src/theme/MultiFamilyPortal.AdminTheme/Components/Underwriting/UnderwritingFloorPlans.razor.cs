namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingFloorPlans
    {
        private bool _showModel { get; set; }
        private int _totalUnits = 0;
        private int _numberOfUnits = 0;
        public IEnumerable<FloorPlan> FloorPlans = Enumerable.Range(1, 10).Select(x => new FloorPlan
        {
            Id = new Guid(),
            Name = "name " + x,
            Bed = new Random().Next(1, x),
            Bath = new Random().Next(1, x),
            SquareFt = new Random().Next(100, x * 100),
            Upgraded = new Random().Next(1, x) == x,
            Units = new Random().Next(1, x),
            Rent = new Random().Next(1000, x * 1000),
            MarketRent = new Random().Next(1000, x * 1000)
        });

        protected override Task OnInitializedAsync()
        {
            _showModel = false;
            GetFloorsAsync();
            // count current
            // count total
            return base.OnInitializedAsync();
        }

        private void GetFloorsAsync()
        {
            // TODO : Get floors specific underwriting
            
        }

        private void OnAddFloor()
        {
            // TODO : Add new Floor support
             _showModel = true;
        }

        private void OnEditFloor()
        {
            // TODO : Update Edit existing floor
            _showModel = true;
        }

        public class FloorPlan
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Bed { get; set; }
            public int Bath { get; set; }
            public int SquareFt { get; set; }
            public bool Upgraded { get; set; }
            public int Units { get; set; }
            public int Rent { get; set; }
            public int MarketRent { get; set; }
        }
    }
}