using ITC.BusinessObject.Identity;

namespace ITC.Services.Email
{
	public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
        Task SendConfirmationEmailAsync(ApplicationUser user, string token);
        Task ResendConfirmationEmailAsync(ApplicationUser user, string token);

        Task SendResetPasswordEmailAsync(ApplicationUser user, string token);
    }

}
