using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class CounterInfo
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string TimeFrame { get; set; }

        [Parameter]
        public string TimeFrameColor { get; set; }

        [Parameter]
        public int Active { get; set; }

        [Parameter]
        public int OfferSubmitted { get; set; }

        [Parameter]
        public int OfferAccepted { get; set; }

        [Parameter]
        public int OfferRejected{ get; set; }

        [Parameter]
        public int Passed { get; set; }

        [Parameter]
        public int LOISubmitted { get; set; }

        [Parameter]
        public int LOIAccepted { get; set; }

        [Parameter]
        public int LOIRejected { get; set; }
    }
}