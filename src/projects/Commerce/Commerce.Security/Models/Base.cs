using System.Security.Cryptography;
using FluentValidation;
using FluentValidation.Results;

namespace Commerce.Security.Models;

public abstract class Base
{
    // User Validation Segment
    private List<string> _errors;
    public IReadOnlyCollection<string> Errors => _errors;
    private void AddErrorList(IList<ValidationFailure> errors)
    {
        foreach (var error in errors)
        {
            _errors.Add(error.ErrorMessage);
        }
    }

    public bool IsValid() => _errors.Count == 0;
    
    protected bool Validate<T, J>(T validator, J obj) where T : AbstractValidator<J>
    {
        var validation = validator.Validate(obj);

        if (validation.Errors.Count > 0)
            AddErrorList(validation.Errors);

        return IsValid();
    }
    
    // Password Hash Segment
    private const int _saltSize = 16;
    private const int _keySize = 32;
    private const int _iterations = 50000;
    private static readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA256;

    private const char segmentDelimiter = ':';
    
    protected string EncryptPassword(string input)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(_saltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, _iterations, _algorithm, _keySize);

        return string.Join(segmentDelimiter, Convert.ToHexString(hash), Convert.ToHexString(salt), _iterations,
            _algorithm);
    }

    protected bool VerifyHash(string input, string hashString)
    {
        string[] segments = hashString.Split(segmentDelimiter);
        byte[] hash = Convert.FromHexString(segments[0]);
        byte[] salt = Convert.FromHexString(segments[1]);
        int iterations = int.Parse(segments[2]);

        HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);

        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, hash.Length);

        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}