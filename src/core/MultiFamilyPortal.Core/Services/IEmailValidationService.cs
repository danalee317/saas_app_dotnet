
namespace MultiFamilyPortal.Services
{
    public interface IEmailValidationService
    {
        Task<EmailValidationResponse> Validate(string email);
    }
}
