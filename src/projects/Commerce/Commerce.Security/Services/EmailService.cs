using Commerce.Security.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Commerce.Security.Services;

public class EmailService : IEmailService
{
    public async Task SendEmailConfirmationAsync(string email, string token)
    {
        var emailSender = new SmtpClient();

        string smtpServer = "smtp.ethereal.email";
        int smtpPort = 587;
        string smtpUser = "narciso.braun@ethereal.email";
        string smtpPass = "MJvgssfUCWx5p2qacs";

        await emailSender.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await emailSender.AuthenticateAsync(smtpUser, smtpPass);

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("N. Braun", smtpUser));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = "Confirm you email";

        string confirmationLink = $"https://localhost:7217/confirm?token={token}";
        emailMessage.Body = new TextPart("plain")
        {
            Text = $"Please confirm your account by clicking this link: {confirmationLink}"
        };

        await emailSender.SendAsync(emailMessage);

        await emailSender.DisconnectAsync(true);
    }
}