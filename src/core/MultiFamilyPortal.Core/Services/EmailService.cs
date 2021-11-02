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

        public async Task<bool> PostPublished(Post post, Uri baseUri)
        {
            bool hasErrors = false;
            var blogContext = _context as IBlogContext;
            if (await blogContext.Posts.Where(x => x.Id == post.Id).SelectMany(x => x.Notifications).AnyAsync())
                return hasErrors;

            var from = new EmailAddress("hello@avantipoint.com", "AvantiPoint Blog");
            var tags = post.Tags.Select(x => new PostNotificationTag { Tag = x.Id });
            var categories = post.Categories.Select(x => new PostNotificationCategory { Category = x.Id });
            var postUri = new Uri(baseUri, $"blog/post/{post.Published.Year}/{post.Published.Month}/{post.Published.Day}/{post.Slug}?utm_source=email&utm_method=subscriber");
            var author = new Dtos.SubscriberNotification {
                AuthorName = post.Author.DisplayName,
                AuthorProfilePic = GravatarHelper.GetUri(post.Author.Email, 80),
                Email = post.Author.Email,
                SocialImage = post.SocialImage,
                SubscribedDate = "Automatically as the content Author",
                Summary = post.Summary,
                Tags = tags,
                Categories = categories,
                Title = post.Title,
                UnsubscribeLink = new Uri(baseUri, $"unsubscribe/{post.Author.Email}?code=author"),
                Url = postUri,
                Year = DateTime.Now.Year
            };
            var subscribers = await blogContext.Subscribers.Where(x => x.IsActive && x.Unsubscribed == null && x.Email != post.Author.Email).ToListAsync();

            var tagTemplate = await _context.EmailPartialTemplates.FirstOrDefaultAsync(x => x.Key == "tag");
            var categoryTemplate = await _context.EmailPartialTemplates.FirstOrDefaultAsync(x => x.Key == "category");
            var subscriberTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(x => x.Key == "subscribernotification");

            Handlebars.RegisterTemplate("tag", tagTemplate.Content);
            Handlebars.RegisterTemplate("category", categoryTemplate.Content);
            Handlebars.RegisterHelper("Summary", RawOutput);
            var template = Handlebars.Compile(subscriberTemplate.Html);
            var htt = new HtmlToText();
            var plainTextSummary = htt.ConvertHtml(post.Summary);
            var subject = $"New Post - {post.Title}";

            try
            {
                var html = template(author);
                var text = string.Format(subscriberTemplate.PlainText, author.Email, author.Title, author.Url, plainTextSummary, author.SubscribedDate, author.UnsubscribeLink);
                var to = new EmailAddress(author.Email);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, text, html);
                var response = await _client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                hasErrors = true;
                _logger.LogError(ex, $"Error emailing author: {post.Author.DisplayName} for post - {post.Title}");
            }

            foreach (var subscriber in subscribers)
            {
                try
                {
                    var notification = new Dtos.SubscriberNotification {
                        AuthorName = post.Author.DisplayName,
                        AuthorProfilePic = GravatarHelper.GetUri(post.Author.Email, 80),
                        Email = subscriber.Email,
                        SocialImage = post.SocialImage,
                        SubscribedDate = post.Published.ToString("D"),
                        Summary = post.Summary,
                        Tags = tags,
                        Categories = categories,
                        Title = post.Title,
                        UnsubscribeLink = new Uri(baseUri, $"unsubscribe/{subscriber.Email}?code={subscriber.UnsubscribeCode()}"),
                        Url = postUri,
                        Year = DateTime.Now.Year
                    };

                    var html = template(notification);
                    var text = string.Format(subscriberTemplate.PlainText, notification.Email, notification.Title, notification.Url, plainTextSummary, notification.SubscribedDate, notification.UnsubscribeLink);

                    if (!await SendAsync(notification.Email, subject, text, html))
                    {
                        hasErrors = true;
                        continue;
                    }

                    subscriber.Notifications.Add(post);
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                    _logger.LogError(ex, $"Error emailing subscriber: {subscriber.Email} for post - {post.Title}");
                }
            }

            return !hasErrors;
        }

        public async Task<bool> SendAsync(string to, string subject, string text, string html)
        {
            var fromEmail = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmail);
            var fromEmailName = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmailFrom);
            var fromAddress = new EmailAddress(fromEmail, fromEmailName);
            var toAddress = new EmailAddress(to);
            return await SendAsync(fromAddress, toAddress, subject, text, html);
        }

        public async Task<bool> SendAsync(EmailAddress to, string subject, string text, string html)
        {
            var fromEmail = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmail);
            var fromEmailName = await _context.GetSettingAsync<string>(PortalSetting.NotificationEmailFrom);
            var fromAddress = new EmailAddress(fromEmail, fromEmailName);
            return await SendAsync(fromAddress, to, subject, text, html);
        }

        public async Task<bool> SendAsync(string from, string to, string subject, string text, string html)
        {
            var fromAddress = new EmailAddress(from);
            var toAddress = new EmailAddress(to);
            return await SendAsync(fromAddress, toAddress, subject, text, html);
        }

        public async Task<bool> SendAsync(EmailAddress from, EmailAddress to, string subject, string text, string html)
        {
            try
            {
                var msg = MailHelper.CreateSingleEmail(from, to, subject, text, html);

#if DEBUG
                System.Diagnostics.Debugger.Break();
                await Task.CompletedTask;
#else
                var response = await _client.SendEmailAsync(msg);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Sendgrid responded with an unexpected response code {response.StatusCode}");
                }
#endif

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email: {to.Email} for regarding - {subject}");
            }

            return false;
        }

        private void RawOutput(EncodedTextWriter output, Context context, Arguments arguments)
        {
            output.WriteSafeString($"{context["Summary"]}");
        }
    }
}
