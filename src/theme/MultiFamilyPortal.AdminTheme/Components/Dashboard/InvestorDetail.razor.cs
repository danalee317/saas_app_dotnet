using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class InvestorDetail
    {
        [Inject]
        private ITimeZoneService _timezoneService { get; set; }

        [Parameter] 
        public bool WindowIsVisible { get; set; }

        [Parameter] 
        public EventCallback<bool> WindowIsVisibleChanged { get; set; }

        [Parameter] 
        public DashboardInvestor Investor { get; set; }

        private string _localTime;

        protected override void OnParametersSet()
        {
            if (Investor == null)
                return;

            string investorTimeZone = HandleTimeZone(Investor.Timezone);

            if (string.IsNullOrEmpty(investorTimeZone))
                _localTime = "Unknown";
            else
                try
                {
                    _localTime = _timezoneService.GetLocalTimeByTimeZone(investorTimeZone).ToShortTimeString();
                }
                catch
                {
                    _localTime = "Unknown";
                }
        }

        private async Task UpdateVisibilty() => await WindowIsVisibleChanged.InvokeAsync(false);

        private string HandleTimeZone(string userInput)
        {
            userInput = userInput.Trim();

            if (string.IsNullOrEmpty(userInput))
                return userInput;

            if (userInput.Length >= 7)
            {
                return _timezoneService.Timezones.FirstOrDefault(x => x.Name.ToLower().Contains(userInput.ToLower()))?.Name;
            }

            var timezone = _timezoneService.Timezones.FirstOrDefault(x => x.Intials == userInput);

            switch (timezone)
            {
                case null when !userInput.Contains("UTC") && !userInput.Contains("GMT"):
                    return "";
                case null:
                    {
                        userInput = userInput.Replace(" ", "");

                        if (userInput.Contains("GMT"))
                        {
                            userInput.Replace("GMT", "UTC");
                        }

                        return _timezoneService.Timezones.FirstOrDefault(x => userInput.Contains(x.Intials))?.Name;
                    }
                default:
                    return timezone.Name;
            }
        }
    }
}
