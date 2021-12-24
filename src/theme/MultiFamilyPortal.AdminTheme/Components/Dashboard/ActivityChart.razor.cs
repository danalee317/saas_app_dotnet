
namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class ActivityChart
    {
        // TODO : Pass AcivityChart properties as parameters
        public class MyPieChartModel
        {
            public string SegmentName { get; set; }
            public double SegmentValue { get; set; }
        }
        protected override void OnParametersSet()
        {
             // TODO : Set ActivityChart properties
             base.OnParametersSet();
        }

        public List<MyPieChartModel> pieData = new List<MyPieChartModel>
    {
        new MyPieChartModel
        {
            SegmentName = "Underwriting",
            SegmentValue = 1
        },
        new MyPieChartModel
        {
            SegmentName = "Brokers",
            SegmentValue = 1
        },
        new MyPieChartModel
        {
            SegmentName = "Management Company",
            SegmentValue = 2
        },
        new MyPieChartModel
        {
            SegmentName = "Lender",
            SegmentValue = 1
        },
        new MyPieChartModel
        {
            SegmentName = "Other",
            SegmentValue = 1
        }
    };
    }
}