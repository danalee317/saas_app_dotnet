using System.ComponentModel.DataAnnotations;

namespace MultiFamilyPortal.ViewModels
{
    public class SubscriberSignupRequest
    {
        [EmailAddress]
        public string? Email { get; set; }
    }
}
