using System.Security.Cryptography;
using Commerce.Security.Interfaces;
using Commerce.Security.Models;
using Commerce.Security.Utils;
using FluentValidation;

namespace Commerce.Security.Services;

public class PasswordService : IPasswordService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 50000;
    private const char SegmentDelimiter = ':';
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public async Task<string> EncryptPasswordAsync(string input)
    {
        byte[] salt = new byte[SaltSize];
        using var rng = RandomNumberGenerator.Create();
        await Task.Run(() => rng.GetBytes(salt));

        byte[]? hash = null;
        await Task.Run(() => hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, Iterations, Algorithm, KeySize));

        return string.Join(SegmentDelimiter, Convert.ToHexString(hash!), Convert.ToHexString(salt), Iterations,
            Algorithm);
    }

    public async Task<User> UpdatePasswordAsync(string passwordStored, User user, string password)
    {
        var hashCheck = VerifyHash(passwordStored, user.HashedPassword);
        if (!hashCheck)
            throw new InvalidPasswordException("A senha antiga está incorreta.");

        var newHash = await UpdateHashAsync(password);

        var newHashCheck = VerifyHash(password, newHash);
        if (!newHashCheck)
            throw new CryptographicException("O novo hash gerado não é compatível com a nova senha.");
        
        user.UpdateHash(newHash);

        return user;
    }

    public bool VerifyHash(string input, string hashPwd)
    {
        string[] segments = hashPwd.Split(SegmentDelimiter);
        byte[] hash = Convert.FromHexString(segments[0]);
        byte[] salt = Convert.FromHexString(segments[1]);
        int iterations = int.Parse(segments[2]);

        HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);

        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, hash.Length);

        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }

    public bool ValidatePassword(string pwd)
    {
        bool isValid = pwd.Any(char.IsUpper)
            && pwd.Any(char.IsLower)
            && pwd.Any(char.IsDigit)
            && pwd.Length >= 8;

        if (!isValid)
            return false;
            
        return isValid;
    }

    private async Task<string> UpdateHashAsync(string newPassword)
    {
        var result = ValidatePassword(newPassword);
        if (!result)
            throw new ValidationException("Password isn't valid.");

        return await EncryptPasswordAsync(newPassword);
    }
}