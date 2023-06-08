using System.Security.Cryptography;
using Commerce.Security.Helpers.Exceptions;
using Commerce.Security.Interfaces;
using Commerce.Security.Models;

namespace Commerce.Security.Services;

public class PasswordService : IPasswordService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 50000;
    private const char SegmentDelimiter = ':';
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
    
    private (bool IsValid, string ErrorMessage) ValidatePassword(string pwd)
    {
        if (!pwd.Any(char.IsUpper))
        {
            return (false, "A senha precisa conter ao menos uma letra maiúscula.");
        }

        if (!pwd.Any(char.IsLower))
        {
            return (false, "A senha precisa conter ao menos uma letra minúscula.");
        }

        if (!pwd.Any(char.IsDigit))
        {
            return (false, "A senha precisa conter ao menos um dígito númerico.");
        }

        if (pwd.Length < 7)
        {
            return (false, "A senha precisa ter tamanho mínimo de 8 digitos contendo minúsculas, maiúsculas e um número.");
        }

        return (true, "Senha válida.");
    }

    public string HashPassword(string input)
    {
        var (isValid, message) = ValidatePassword(input);
        if (!isValid)
        {
            throw new InvalidPasswordException(message);
        }
        byte[] salt = new byte[SaltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        byte[]? hash;
        hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, Iterations, Algorithm, KeySize);

        return string.Join(SegmentDelimiter, Convert.ToHexString(hash), Convert.ToHexString(salt), Iterations,
            Algorithm);
    }

    public bool ValidateHash(string input, string hashPwd)
    {
        string[] segments = hashPwd.Split(SegmentDelimiter);
        byte[] hash = Convert.FromHexString(segments[0]);
        byte[] salt = Convert.FromHexString(segments[1]);
        int iterations = int.Parse(segments[2]);

        HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);

        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, hash.Length);

        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
    
    public User UpdateHash(string passwordStored, User user, string password)
    {
        var isHashValid = ValidateHash(passwordStored, user.HashedPassword);
        if (!isHashValid)
        {
            throw new InvalidPasswordException("A senha antiga está incorreta.");
        }

        string newHash = HashPassword(password);
        user.HashedPassword = newHash;
        return user;
    }
}