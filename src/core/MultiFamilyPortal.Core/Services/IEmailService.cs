using MultiFamilyPortal.Data.Models;
using SendGrid.Helpers.Mail;

namespace MultiFamilyPortal.Services
{
    public interface IEmailService
    {
        Task<bool> PostPublished(Post post, Uri baseUri);
        Task<bool> SendAsync(EmailAddress from, EmailAddress to, string subject, string text, string html);
        Task<bool> SendAsync(EmailAddress to, string subject, string text, string html);
        Task<bool> SendAsync(string to, string subject, string text, string html);
        Task<bool> SendAsync(string from, string to, string subject, string text, string html);
    }
}
