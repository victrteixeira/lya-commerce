using Commerce.Security.Models;

namespace Commerce.Security.Interfaces;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(User user, string token);
    Task SendEmailForgotPasswordAsync(User user, string token);
}