using Commerce.Security.DTOs;

namespace Commerce.Security.Interfaces;

public interface ISecurityService
{
    Task<ReadUser?> RegisterAsync(CreateUser command);
    Task<bool> LoginAsync(LoginUser command);
}