using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MultiFamilyPortal.Services
{
    internal class EmailService : IEmailService
    {
        private ILogger<EmailService> _logger { get; }
        private ISendGridClient _client { get; }
        private IMFPContext _context { get; }

        public EmailService(ISendGridClient client, IMFPContext context, ILoggerFactory loggerFactory)
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

            return await SendAsync(new EmailAddress(fromEmail, fromEmailName), new EmailAddress(toEmail, fromEmailName), template);
        }

        public async Task<bool> SendAsync(string to, TemplateResult template)
        {
            var fromEmail = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmail);
            var fromEmailName = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmailFrom);
            var fromAddress = new EmailAddress(fromEmail, fromEmailName);
            var toAddress = new EmailAddress(to);
            return await SendAsync(fromAddress, toAddress, template);
        }

        public async Task<bool> SendAsync(EmailAddress to, TemplateResult template)
        {
            var fromEmail = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmail);
            var fromEmailName = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmailFrom);
            var fromAddress = new EmailAddress(fromEmail, fromEmailName);
            return await SendAsync(fromAddress, to, template);
        }

        public async Task<bool> SendAsync(string from, string to, TemplateResult template)
        {
            var fromAddress = new EmailAddress(from);
            var toAddress = new EmailAddress(to);
            return await SendAsync(fromAddress, toAddress, template);
        }

        public async Task<bool> SendAsync(EmailAddress from, EmailAddress to, TemplateResult template)
        {
            try
            {
                var msg = MailHelper.CreateSingleEmail(from, to, template.Subject, template.PlainText, template.Html);
                var response = await _client.SendEmailAsync(msg);

                if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogWarning($"Email to: '{to}' regarding - '{template.Subject}' was not successful.");
                    var body = await response.Body.ReadAsStringAsync();
                    _logger.LogWarning($"{response.StatusCode} ({(int)response.StatusCode}) - {body}");
                    throw new Exception($"Sendgrid responded with an unexpected response code {response.StatusCode}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email: {to.Email} for regarding - {template.Subject}");
            }

            return false;
        }

        private void RawOutput(EncodedTextWriter output, Context context, Arguments arguments)
        {
            output.WriteSafeString($"{context["Summary"]}");
        }
    }
}
