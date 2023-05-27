namespace Commerce.Security.Interfaces;

public interface IPasswordService
{
    Task<string> EncryptPassword(string input);
    bool VerifyHash(string input, string hashPwd);
    bool ValidatePassword(string pwd);
}