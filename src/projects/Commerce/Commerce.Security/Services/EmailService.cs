using Commerce.Security.Interfaces;
using Commerce.Security.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;

namespace Commerce.Security.Services;

public class EmailService : IEmailService
{
    private readonly IMemoryCache _cache;
    public EmailService(IMemoryCache cache) => _cache = cache;

    public async Task SendEmailConfirmationAsync(User user, string token)
    {
        var emailSender = new SmtpClient();
        
        string smtpServer = "smtp.mailgun.org";
        int smtpPort = 587;
        string smtpUser = "postmaster@sandbox2620912c6c774a4cb221273741eac080.mailgun.org";
        string smtpPass = "";
        
        await emailSender.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await emailSender.AuthenticateAsync(smtpUser, smtpPass);
        
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Cat Moon MakeUp", smtpUser));
        emailMessage.To.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", user.EmailAddress));
        emailMessage.Subject = "Confirm you email";
        
        string confirmationLink = $"https://localhost:7217/api/v1/security/confirm?token={token}";
        emailMessage.Body = new TextPart("plain")
        {
            Text = $"Please confirm your account by clicking this link: {confirmationLink}"
        };
        
        await emailSender.SendAsync(emailMessage);
        await emailSender.DisconnectAsync(true);
        _cache.Set(token, TimeSpan.FromMinutes(10));
    }

    public async Task SendEmailForgotPasswordAsync(User user, string token)
    {
        var emailSender = new SmtpClient();
        
        string smtpServer = "smtp.mailgun.org";
        int smtpPort = 587;
        string smtpUser = "postmaster@sandbox2620912c6c774a4cb221273741eac080.mailgun.org";
        string smtpPass = "";
        
        await emailSender.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await emailSender.AuthenticateAsync(smtpUser, smtpPass);
        
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Cat Moon MakeUp", smtpUser));
        emailMessage.To.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", user.EmailAddress));
        emailMessage.Subject = "Confirm you email";
        
        string confirmationLink = $"https://localhost:7217/api/v1/security/password/recovery?token={token}";
        emailMessage.Body = new TextPart("plain")
        {
            Text = $"Please confirm your account by clicking this link: {confirmationLink}"
        };
        
        await emailSender.SendAsync(emailMessage);
        await emailSender.DisconnectAsync(true);
        _cache.Set(token, TimeSpan.FromMinutes(10));
    }
}