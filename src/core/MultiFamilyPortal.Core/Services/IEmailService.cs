using SendGrid.Helpers.Mail;

namespace MultiFamilyPortal.Services
{
    public interface IEmailService
    {
        Task<bool> SendAsync(TemplateResult template);
        Task<bool> SendAsync(EmailAddress from, EmailAddress to, TemplateResult template);
        Task<bool> SendAsync(EmailAddress to, TemplateResult template);
        Task<bool> SendAsync(string to, TemplateResult template);
        Task<bool> SendAsync(string from, string to, TemplateResult template);
    }
}
