using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class InvestorDetail
    {
        [Parameter]
        public bool WindowIsVisible { get; set; }

        [Parameter]
        public EventCallback<bool> WindowIsVisibleChanged { get; set; }

        [Parameter]
        public InvestorProspect Investor { get; set; }

        private string _localTime;

        protected override void OnParametersSet()
        {
            if (Investor == null)
                return;

            var timezoneOffset = TimeZoneInfo.Local.GetUtcOffset(Investor.Timestamp);
            Console.WriteLine($"Timezone Offset: {timezoneOffset} {Investor.Timestamp}");
            _localTime = DateTimeOffset.UtcNow.Add(timezoneOffset).ToString("hh:mm tt");
        }

        private async Task UpdateVisibilty() => await WindowIsVisibleChanged.InvokeAsync(false);

    }
}