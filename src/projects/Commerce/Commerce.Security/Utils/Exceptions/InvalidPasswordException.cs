namespace Commerce.Security.Utils;

public class InvalidPasswordException : Exception
{
    public InvalidPasswordException()
    {
    }

    public InvalidPasswordException(string? message) : base(message)
    {
    }
}