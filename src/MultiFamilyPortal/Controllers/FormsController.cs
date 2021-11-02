using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("/api/[controller]")]
    public class FormsController : ControllerBase
    {
        private IEmailService _emailService { get; }
        private IEmailValidationService _emailValidator { get; }
        private IMFPContext _dbContext { get; }
        private IIpLookupService _ipLookup { get; }


        [HttpPost("contact-us")]
        public async Task<IActionResult> ContactUs([FromBody] ContactFormRequest form)
        {
            var validatorResponse = await _emailValidator.Validate(form.Email);

            if (validatorResponse.IsValid)
                return BadRequest(validatorResponse.Message);



            return Ok();
        }

        [HttpPost("investor-inquiry")]
        public async Task<IActionResult> InvestorInquiry([FromBody]InvestorInquiryRequest form)
        {
            var validatorResponse = await _emailValidator.Validate(form.Email);

            if (validatorResponse.IsValid)
                return BadRequest(validatorResponse.Message);

            return Ok();
        }

        [HttpPost("newsletter-subscriber")]
        public async Task<IActionResult> NewsletterSubscriber([FromBody]NewsletterSubscriberRequest form)
        {
            var validatorResponse = await _emailValidator.Validate(form.Email);

            if (validatorResponse.IsValid)
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
                IsActive = true,
                Region = ipData.Region,
            };
            blogContext.Subscribers.Add(subscriber);
            await blogContext.SaveChangesAsync();

            // TODO: Send Notification
            

            return Ok();
        }
    }
}
