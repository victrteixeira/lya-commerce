namespace Commerce.Security.Utils;

public class UserException : Exception
{
    public UserException()
    {
    }

    public UserException(string? message) : base(message)
    {
    }
}