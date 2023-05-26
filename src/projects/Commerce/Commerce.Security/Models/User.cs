using System.Security.Cryptography;
using Commerce.Security.Validators;
using FluentValidation;

namespace Commerce.Security.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string EmailAddress { get; private set; }
    public string Password { get; private set; }
    public string HashedPassword { get; private set; }
    
    public User(string firstname, string lastname, string emailAddress, string password)
    {
        FirstName = firstname;
        LastName = lastname;
        EmailAddress = emailAddress;
        Password = password.Trim();
        ValidateUser();
        HashedPassword = EncryptPassword(password);
    }
    
    private void ValidateUser() => new UserValidator().ValidateAndThrow(this);
    
    // Password Hash Segment
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 50000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    private const char SegmentDelimiter = ':';
    
    private string EncryptPassword(string input)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, Iterations, Algorithm, KeySize);

        return string.Join(SegmentDelimiter, Convert.ToHexString(hash), Convert.ToHexString(salt), Iterations,
            Algorithm);
    }

    private bool VerifyHash(string input, string hashString)
    {
        string[] segments = hashString.Split(SegmentDelimiter);
        byte[] hash = Convert.FromHexString(segments[0]);
        byte[] salt = Convert.FromHexString(segments[1]);
        int iterations = int.Parse(segments[2]);

        HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);

        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, hash.Length);

        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}