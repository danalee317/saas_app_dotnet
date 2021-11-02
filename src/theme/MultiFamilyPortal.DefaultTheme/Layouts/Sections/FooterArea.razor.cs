using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.DefaultTheme.Layouts.Sections
{
    public partial class FooterArea
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; } = default!;

        [Inject]
        private HttpClient _client { get; set; } = default!;

        [Inject]
        private IEmailValidationService EmailValidator { get; set; }

        //[Inject]
        //private IHttpContextAccessor ContextAccessor { get; set; }

        private PortalNotification notification { get; set; } = default!;
        private ServerSideValidator serverSideValidator { get; set; }

        private NewsletterSubscriberRequest SignupModel { get; set; } = new();
        private bool subscribed;
        private async Task OnValidSignupRequest(EditContext context)
        {
            var validationResponse = await EmailValidator.Validate(SignupModel.Email);
            if (!validationResponse.IsValid)
            {
                // Add validation error
                serverSideValidator.DisplayErrors(new Dictionary<string, List<string>> { { nameof(NewsletterSubscriberRequest.Email), new List<string> { validationResponse.Message } } });
                return;
            }

            using var response = await _client.PostAsJsonAsync("/api/forms/newsletter-subscriber", SignupModel);

            notification.ShowSuccess("You have subscribed to our updates.");
            //var subscriber = new Subscriber{IsActive = true, Email = SignupModel.Email, IpAddress = ContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(), };
            //SiteContext.Subscribers.Add(subscriber);
            //await SiteContext.SaveChangesAsync();
        }
    }
}
