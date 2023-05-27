namespace Commerce.Security.Utils;

public class LoginException : Exception
{
    public LoginException()
    {
    }

    public LoginException(string? message) : base(message)
    {
    }
}