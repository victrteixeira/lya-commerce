using Commerce.Security.Models;

namespace Commerce.Security.Interfaces;

public interface IPasswordService
{
    string HashPassword(string input);
    bool ValidateHash(string input, string hashPwd);
    User UpdateHash(string passwordStored, User user, string password);
}
