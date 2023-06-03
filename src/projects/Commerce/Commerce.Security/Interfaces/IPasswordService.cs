using Commerce.Security.Models;

namespace Commerce.Security.Interfaces;

public interface IPasswordService
{
    Task<string> EncryptPasswordAsync(string input);
    Task<User> UpdatePasswordAsync(string passwordStored, User user, string password);
    bool VerifyHash(string input, string hashPwd);
    bool ValidatePassword(string pwd);
}