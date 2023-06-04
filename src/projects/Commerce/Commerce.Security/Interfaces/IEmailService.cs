namespace Commerce.Security.Interfaces;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(string email, string token);
}