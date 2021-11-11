using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.DefaultTheme.Pages
{
    public partial class InvestWithUs
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; }

        [Inject]
        private IFormService _formService { get; set; }

        private InvestorInquiryRequest _form => _formService?.InvestorInquiry;
        private PortalNotification notification { get; set; } = default!;
        private ServerSideValidator serverSideValidator { get; set; } = default!;
        private bool submitted;

        private async Task SubmitForm()
        {
            var response = await _formService.SubmitSubscriberSignup();
            if (response?.Errors?.Any() ?? false)
            {
                serverSideValidator.DisplayErrors(response.Errors);
            }

            notification.Show(response);
            submitted = response.State == ResultState.Success;
        }
    }
}
