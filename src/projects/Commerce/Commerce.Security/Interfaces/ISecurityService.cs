using Commerce.Security.DTOs;

namespace Commerce.Security.Interfaces;

public interface ISecurityService
{
    Task<ReadUser?> RegisterAsync(CreateUser command);
    Task<bool> ConfirmEmailAsync(string token);
    Task<string?> LoginAsync(LoginUser command);
    Task<bool> ChangePasswordAsync(ChangePasswordUser command);
    Task ForgotPasswordRequest(string? email);
    Task<bool> PasswordRecovery(string token, ResetPasswordUser command);
}