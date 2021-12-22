using System.Net.Mail;

namespace MultiFamilyPortal.Services
{
    public interface IEmailService
    {
        Task<bool> SendAsync(TemplateResult template);
        Task<bool> SendAsync(MailAddress from, MailAddress to, TemplateResult template);
        Task<bool> SendAsync(MailAddress to, TemplateResult template);
        Task<bool> SendAsync(string to, TemplateResult template);
        Task<bool> SendAsync(string from, string to, TemplateResult template);
    }
}
