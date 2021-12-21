using System.Net.Mail;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Data;
using PostmarkDotNet;

namespace MultiFamilyPortal.Services
{
    internal class EmailService : IEmailService
    {
        private ILogger<EmailService> _logger { get; }
        private PostmarkClient _client { get; }
        private IMFPContext _context { get; }

        public EmailService(PostmarkClient client, IMFPContext context, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EmailService>();
            _client = client;
            _context = context;
        }

        public async Task<bool> SendAsync(TemplateResult template)
        {
            var fromEmail = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmail);
            var fromEmailName = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmailFrom);
            var toEmail = await _context.GetSettingAsync<string>(PortalSetting.ContactEmail);

            return await SendAsync(new MailAddress(fromEmail, fromEmailName), new MailAddress (toEmail, fromEmailName), template);
        }

        public async Task<bool> SendAsync(string to, TemplateResult template)
        {
            var fromEmail = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmail);
            var fromEmailName = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmailFrom);
            var fromAddress = new MailAddress(fromEmail, fromEmailName);
            var toAddress = new MailAddress(to);
            return await SendAsync(fromAddress, toAddress, template);
        }

        public async Task<bool> SendAsync(MailAddress to, TemplateResult template)
        {
            var fromEmail = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmail);
            var fromEmailName = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmailFrom);
            var fromAddress = new MailAddress(fromEmail, fromEmailName);
            return await SendAsync(fromAddress, to, template);
        }

        public async Task<bool> SendAsync(string from, string to, TemplateResult template)
        {
            var fromAddress = new MailAddress(from);
            var toAddress = new MailAddress(to);
            return await SendAsync(fromAddress, toAddress, template);
        }

        public async Task<bool> SendAsync(MailAddress from, MailAddress to, TemplateResult template)
        {
            try
            {
                using var message = new MailMessage(from, to)
                {
                    Body = template.Html,
                    BodyEncoding = System.Text.Encoding.Default,
                    IsBodyHtml = true,
                    Subject = template.Subject,
                };

                var response = await _client.SendMessageAsync(from.ToString(), to.ToString(), template.Subject, template.PlainText, template.Html);

                if (response.Status != PostmarkStatus.Success)
                {
                    _logger.LogWarning($"Email to: '{to}' regarding - '{template.Subject}' was not successful.");
                    _logger.LogWarning($"Postmark responded with an unexpected response code {response.Status} - Error Code: {response.ErrorCode} - Message: {response.Message}");
                    throw new Exception($"Postmark responded with an unexpected response code {response.Status} - Error Code: {response.ErrorCode} - Message: {response.Message}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email: {to} for regarding - {template.Subject}");
            }

            return false;
        }

        private void RawOutput(EncodedTextWriter output, Context context, Arguments arguments)
        {
            output.WriteSafeString($"{context["Summary"]}");
        }
    }
}
