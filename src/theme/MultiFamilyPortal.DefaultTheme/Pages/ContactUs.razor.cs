using System.Reactive;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.DefaultTheme.Pages
{
    public partial class ContactUs
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; }

        [Inject]
        private IFormService _formService { get; set; }

        private ContactFormRequest form => _formService?.ContactForm;
        private PortalNotification notification { get; set; } = default!;
        private ServerSideValidator serverSideValidator { get; set; } = default!;
        private bool submitted;

        private async Task SubmitForm()
        {
            var response = await _formService.SubmitContactForm();
            if (response?.Errors?.Any() ?? false)
            {
                            serverSideValidator.DisplayErrors(response.Errors);
                return;
            }

            notification.Show(response);
            submitted = true;
        }
    }
}
