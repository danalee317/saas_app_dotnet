using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.ComponentModel;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.Services
{
    internal class DatabaseTemplateProvider : ITemplateProvider
    {
        private const string HtmlTemplateHead = @"<!DOCTYPE html>
<html>
<head>
  <meta charset=""UTF-8"" />
  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
  <title>{{SiteTitle}} - {{Subject}}</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />

  <link rel=""shortcut icon"" href=""{{SiteUrl}}/favicon.ico"" type=""image/x-icon"" />
  <link rel=""icon"" type=""image/png"" sizes=""32x32"" href=""{{SiteUrl}}/favicon-32x32.png"" />
  <link rel=""icon"" type=""image/png"" sizes=""16x16"" href=""{{SiteUrl}}/favicon-16x16.png"" />
  <link rel=""manifest"" href=""{{SiteUrl}}/manifest.json"" />

  <!-- stylesheets -->
  <link rel=""stylesheet"" type=""text/css"" href=""https://ap-corp-site.azurewebsites.net/css/bootstrap.css"" />
  <link rel=""stylesheet"" type=""text/css"" href=""https://ap-corp-site.azurewebsites.net/css/theme.min.css"" />

  <!-- javascript -->
  <script src=""https://ap-corp-site.azurewebsites.net/js/theme.min.js""></script>

  <!--[if lt IE 9]>
    <script src=""http://html5shim.googlecode.com/svn/trunk/html5.js""></script>
  <![endif]-->
</head>
<body>
";
        private IMFPContext _context { get; }
        private ISiteInfo _siteInfo { get; }
        private IHttpContextAccessor _contextAccessor { get; }

        public DatabaseTemplateProvider(IMFPContext context, ISiteInfo siteInfo, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _siteInfo = siteInfo;
        }

        public async Task<TemplateResult> GetTemplate<T>(string templateName, T model)
            where T : HtmlTemplateBase
        {
            var type = typeof(T);
            if(_contextAccessor.HttpContext != null)
            {
                var request = _contextAccessor.HttpContext.Request;
                model.SiteUrl = $"{request.Scheme}://{request.Host}";
            }

            model.SiteTitle = _siteInfo.Title;

            var partials = type.GetCustomAttributes<PartialTemplateAttribute>();
            var properties = type.GetRuntimeProperties();
            var emailTemplate = await GetTemplate(templateName);

            if(partials.Any())
            {
                foreach(var partial in partials)
                {
                    var partialTemplate = await GetPartialTemplate(partial.Name);
                    Handlebars.RegisterTemplate(partial.Name, partialTemplate);
                }
            }

            foreach(var rawProperty in properties.Where(x => x.GetCustomAttributes<RawOutputAttribute>().Any()))
            {
                Handlebars.RegisterHelper(rawProperty.Name, (output, context, args) =>
                    output.WriteSafeString($"{context[rawProperty.Name]}"));
            }

            var templateHtml = $"{HtmlTemplateHead}{emailTemplate.Html}\n</body>\n</html>";
            var htmlTemplate = Handlebars.Compile(templateHtml);

            var html = htmlTemplate(model);
            var htt = new HtmlToText();

            foreach(var plainTextProperty in properties.Where(x => x.GetCustomAttributes<PlainTextAttribute>().Any()))
            {
                var value = plainTextProperty.GetValue(model).ToString();
                plainTextProperty.SetValue(model, htt.ConvertHtml(value));
            }

            var text = ReplaceTokens(emailTemplate.PlainText, model);

            return new TemplateResult
            {
                Subject = model.Subject,
                Html = html,
                PlainText = text
            };
        }

        public async Task<TemplateResult> GetSubscriberNotification(SubscriberNotification notification)
        {
            var emailTemplate = await GetTemplate("subscribernotification");
            Handlebars.RegisterHelper("Summary", SummaryRawOutput);
            Handlebars.RegisterTemplate("tag", await GetPartialTemplate("tag"));
            Handlebars.RegisterTemplate("category", await GetPartialTemplate("category"));
            var htmlTemplate = Handlebars.Compile(emailTemplate.Html);

            var html = htmlTemplate(notification);
            var htt = new HtmlToText();
            notification.Summary = htt.ConvertHtml(notification.Summary);
            var text = ReplaceTokens(emailTemplate.PlainText, notification);

            return new TemplateResult
            {
                Subject = notification.Subject,
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
            notification.Message = htt.ConvertHtml(notification.Message);
            var text = ReplaceTokens(emailTemplate.PlainText, notification);

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

        private static void ReplaceValue(ref string text, string name, string value)
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

        private static string ReplaceTokens<T>(in string text, T model)
        {
            var props = typeof(T).GetRuntimeProperties()
                .Where(x => !typeof(IEnumerable).IsAssignableFrom(x.PropertyType))
                .ToDictionary(x => x.Name, x => GetFormattedValue(x, model));

            var output = text;
            foreach((var name, var value) in props)
            {
                ReplaceValue(ref output, name, value);
            }

            return output;
        }

        private static string GetFormattedValue(PropertyInfo propertyInfo, object model)
        {
            var value = propertyInfo.GetValue(model, null);
            var displayFormat = propertyInfo.GetCustomAttribute<DisplayFormatAttribute>();
            if (displayFormat != null && !string.IsNullOrEmpty(displayFormat.DataFormatString))
                return string.Format(displayFormat.DataFormatString, value);

            return value.ToString();
        }
    }
}
