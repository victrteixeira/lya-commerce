namespace Commerce.Security.Interfaces;

public interface IPasswordService
{
    Task<string> EncryptPasswordAsync(string input);
    bool VerifyHash(string input, string hashPwd);
    bool ValidatePassword(string pwd);
}