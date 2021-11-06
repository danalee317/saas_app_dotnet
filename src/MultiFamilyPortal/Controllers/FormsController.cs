using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;
using SendGrid.Helpers.Mail;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("/api/[controller]")]
    public class FormsController : ControllerBase
    {
        private IEmailService _emailService { get; }
        private IEmailValidationService _emailValidator { get; }
        private ITemplateProvider _templateProvider { get; }
        private IMFPContext _dbContext { get; }
        private IIpLookupService _ipLookup { get; }
        private ISiteInfo _siteInfo { get; }

        public FormsController(IMFPContext context,
            IEmailService emailService,
            IEmailValidationService emailValidationService,
            IIpLookupService ipLookupService,
            ISiteInfo siteInfo,
            ITemplateProvider templateProvider)
        {
            _dbContext = context;
            _emailService = emailService;
            _emailValidator = emailValidationService;
            _ipLookup = ipLookupService;
            _siteInfo = siteInfo;
            _templateProvider = templateProvider;
        }


        [HttpPost("contact-us")]
        public async Task<IActionResult> ContactUs([FromBody] ContactFormRequest form)
        {
            var validatorResponse = await _emailValidator.Validate(form.Email);

            if (validatorResponse.IsValid)
                return BadRequest(validatorResponse.Message);

            var url = $"{Request.Scheme}://{Request.Host}";
            var notification = new ContactFormEmailNotification
            {
                DisplayName = form.Email,
                Email = form.Email,
                FirstName = form.Email,
                LastName = form.Email,
                Message = $"<p>Thank you for contacting us. One of our team members will be in touch shortly.</p>",
                SiteTitle = _siteInfo.Title,
                SiteUrl = url,
                Subject = $"Investor Request {_siteInfo.Title}",
                Year = DateTime.Now.Year
            };
            var message = await _templateProvider.GetTemplate(PortalTemplate.ContactForm, notification);
            var emailAddress = new EmailAddress(form.Email, $"{form.FirstName} {form.LastName}".Trim());
            await _emailService.SendAsync(emailAddress, message);

            // TODO: Send email to site admins

            return Ok();
        }

        [HttpPost("investor-inquiry")]
        public async Task<IActionResult> InvestorInquiry([FromBody]InvestorInquiryRequest form)
        {
            var validatorResponse = await _emailValidator.Validate(form.Email);

            if (validatorResponse.IsValid)
                return BadRequest(validatorResponse.Message);

            await _dbContext.InvestorProspects.AddAsync(new InvestorProspect
            {
                Email = form.Email,
                FirstName = form.FirstName,
                LastName = form.LastName,
                LookingToInvest = form.LookingToInvest,
                Phone = form.Phone,
                Timezone = form.Timezone,
            });

            await _dbContext.SaveChangesAsync();

            var url = $"{Request.Scheme}://{Request.Host}";
            var notification = new ContactFormEmailNotification
            {
                DisplayName = form.Email,
                Email = form.Email,
                FirstName = form.Email,
                LastName = form.Email,
                Message = $"<p>Thank you for contacting us. One of our team members will be in touch shortly.</p>",
                SiteTitle = _siteInfo.Title,
                SiteUrl = url,
                Subject = $"Investor Request {_siteInfo.Title}",
                Year = DateTime.Now.Year
            };
            var message = await _templateProvider.GetTemplate(PortalTemplate.ContactForm, notification);
            var emailAddress = new EmailAddress(form.Email, $"{form.FirstName} {form.LastName}".Trim());
            await _emailService.SendAsync(emailAddress, message);

            // TODO: Send email to site admins

            return Ok();
        }

        [HttpPost("newsletter-subscriber")]
        public async Task<IActionResult> NewsletterSubscriber([FromBody]NewsletterSubscriberRequest form)
        {
            var validatorResponse = await _emailValidator.Validate(form.Email);

            if (!validatorResponse.IsValid)
                return BadRequest(validatorResponse.Message);

            var blogContext = _dbContext as IBlogContext;
            if (await blogContext.Subscribers.AnyAsync(x => x.Email == form.Email))
                return Ok();

            var ipData = await _ipLookup.LookupAsync(HttpContext.Connection.RemoteIpAddress, Request.Host.Value);

            var subscriber = new Subscriber {
                City = ipData.City,
                Continent = ipData.Continent,
                Email = form.Email,
                IpAddress = HttpContext.Connection.RemoteIpAddress,
                Country = ipData.Country,
                Region = ipData.Region,
            };
            blogContext.Subscribers.Add(subscriber);
            await blogContext.SaveChangesAsync();

            var url = $"{Request.Scheme}://{Request.Host}";
            var confirmationUrl = $"{url}/subscriber/confirmation/{subscriber.ConfirmationCode}";

            var notification = new ContactFormEmailNotification
            {
                DisplayName = form.Email,
                Email = form.Email,
                FirstName = form.Email,
                LastName = form.Email,
                Message = $"<p>Thank you for signing up!<br />Before we start sending you messages, please confirm that this email address belongs to you and that you would like to recieve messages from us. Don't worry, if you didn't sign up, you won't get anything from us unless your confirm you email address.</p><div class=\"text-center\"><p><a href=\"{confirmationUrl}\">Confirm Email</a><br />Link not working? Copy and paste this url into your browser: {confirmationUrl}</p>",
                SiteTitle = _siteInfo.Title,
                SiteUrl = url,
                Subject = $"Successfully subscribed to updates on {_siteInfo.Title}",
                Year = DateTime.Now.Year
            };
            var message = await _templateProvider.GetTemplate(PortalTemplate.ContactForm, notification);
            await _emailService.SendAsync(form.Email, message);

            return Ok();
        }
    }
}
