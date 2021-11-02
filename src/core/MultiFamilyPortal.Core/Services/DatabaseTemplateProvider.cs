using System.Text.RegularExpressions;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.Services
{
    internal class DatabaseTemplateProvider : ITemplateProvider
    {
        private IMFPContext _context { get; }

        public DatabaseTemplateProvider(IMFPContext context)
        {
            _context = context;
        }

        public async Task<TemplateResult> GetSubscriberNotification(SubscriberNotification notification)
        {
            var emailTemplate = await GetTemplate("subscribernotification");
            Handlebars.RegisterHelper("Summary", SummaryRawOutput);
            Handlebars.RegisterTemplate("tag", await GetPartialTemplate("tag"));
            Handlebars.RegisterTemplate("category", await GetPartialTemplate("category"));
            var template = Handlebars.Compile(emailTemplate.Html);

            var html = template(notification);
            var htt = new HtmlToText();
            var plainTextSummary = htt.ConvertHtml(notification.Summary);
            var text = string.Format(emailTemplate.PlainText, notification.Email, notification.Title, notification.Url, plainTextSummary, notification.SubscribedDate, notification.UnsubscribeLink);

            return new TemplateResult
            {
                Html = html,
                PlainText = text
            };
        }

        public async Task<TemplateResult> ContactUs(ContactFormEmailNotification notification)
        {
            var emailTemplate = await GetTemplate("contact-form");
            Handlebars.RegisterHelper("Message", MessageRawOutput);
            var template = Handlebars.Compile(emailTemplate.Html);

            var html = template(notification);
            var htt = new HtmlToText();
            var plainTextMessage = htt.ConvertHtml(notification.Message);
            var text = emailTemplate.PlainText;
            ReplaceValue(ref text, nameof(ContactFormEmailNotification.DisplayName), notification.DisplayName);
            ReplaceValue(ref text, nameof(ContactFormEmailNotification.Email), notification.Email);
            ReplaceValue(ref text, nameof(ContactFormEmailNotification.FirstName), notification.FirstName);
            ReplaceValue(ref text, nameof(ContactFormEmailNotification.LastName), notification.LastName);
            ReplaceValue(ref text, nameof(ContactFormEmailNotification.Message), plainTextMessage);
            ReplaceValue(ref text, nameof(ContactFormEmailNotification.SiteTitle), notification.SiteTitle);
            ReplaceValue(ref text, nameof(ContactFormEmailNotification.SiteUrl), notification.SiteUrl);
            ReplaceValue(ref text, nameof(ContactFormEmailNotification.Subject), notification.Subject);
            ReplaceValue(ref text, nameof(ContactFormEmailNotification.Year), notification.Year.ToString());

            return new TemplateResult
            {
                Html = html,
                PlainText = text
            };
        }

        private void SummaryRawOutput(EncodedTextWriter output, Context context, Arguments arguments)
        {
            output.WriteSafeString($"{context["Summary"]}");
        }

        private void MessageRawOutput(EncodedTextWriter output, Context context, Arguments arguments)
        {
            output.WriteSafeString($"{context["Message"]}");
        }

        private void ReplaceValue(ref string text, string name, string value)
        {
            var pattern = Regex.Escape($"{{{{{name}}}}}");
            text = Regex.Replace(text, pattern, value);
        }

        private async Task<string> GetPartialTemplate(string key)
        {
            var template = await _context.EmailPartialTemplates.FirstOrDefaultAsync(x => x.Key == key);
            return template?.Content;
        }

        private async Task<EmailTemplate> GetTemplate(string key)
        {
            var template = await _context.EmailTemplates.FirstOrDefaultAsync(x => x.Key == key);
            return template;
        }
    }
}
