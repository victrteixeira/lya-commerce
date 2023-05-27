using System.Security.Cryptography;
using Commerce.Security.Interfaces;
using Commerce.Security.Utils;

namespace Commerce.Security.Services;

public class PasswordService : IPasswordService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 50000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    private const char SegmentDelimiter = ':';
    
    public async Task<string> EncryptPassword(string input)
    {
        byte[] salt = new byte[SaltSize];
        await Task.Run(() => RandomNumberGenerator.GetBytes(SaltSize));

        byte[]? hash = null;
        await Task.Run(() => hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, Iterations, Algorithm, KeySize));

        return string.Join(SegmentDelimiter, Convert.ToHexString(hash!), Convert.ToHexString(salt), Iterations,
            Algorithm);
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
}