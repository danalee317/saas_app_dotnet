using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ARSoft.Tools.Net.Dns;

namespace MultiFamilyPortal.Services
{
    internal class EmailValidationService : IEmailValidationService
    {
        private EmailValidationOptions _options { get; }

        //public EmailValidationService(EmailValidationOptions options)
        //{
        //    _options = options;
        //}

        public async Task<EmailValidationResponse> Validate(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                if (!mailAddress.Host.Contains(".") || mailAddress.Host.EndsWith(".local"))
                    return new EmailValidationResponse { Message = "The specified email is a local domain address" };

                var domain = mailAddress.Host;
                var resolver = new DnsStubResolver();
                var records = resolver.Resolve<MxRecord>(domain, RecordType.Mx);

                if (!records.Any())
                    return new EmailValidationResponse { Message = "No Mx Records could be located for the given email address" };
                //using var client = new HttpClient();
                //var urlTemplate = $"https://emailverification.whoisxmlapi.com/api/v1?apiKey={_options.Key}&emailAddress={{0}}";
                //var apiResponse = await client.GetFromJsonAsync<WhoisXmlApiEmailValidationResponse>(string.Format(urlTemplate, email));

                //if(apiResponse.)

                await Task.CompletedTask;
                return new EmailValidationResponse { IsValid = true };
            }
            catch
            {
                return new EmailValidationResponse { Message = "The Email is an incorrect format" };
            }
        }
    }

    internal class WhoisXmlApiEmailValidationResponse
    {
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("formatCheck")]
        public bool FormatCheck { get; set; }

        [JsonPropertyName("smtpCheck")]
        public bool SmtpCheck { get; set; }

        [JsonPropertyName("dnsCheck")]
        public bool DnsCheck { get; set; }

        [JsonPropertyName("freeCheck")]
        public bool FreeCheck { get; set; }

        [JsonPropertyName("disposableCheck")]
        public bool DisposableCheck { get; set; }

        [JsonPropertyName("catchAllCheck")]
        public bool CatchAllCheck { get; set; }

        [JsonPropertyName("mxRecords")]
        public string[] MxRecords { get; set; }
    }

    public record EmailValidationResponse
    {
        public bool IsValid { get; init; }
        public string Message { get; init; }
    }

    public class EmailValidationOptions
    {
        public string Key { get; set; }
    }
}
