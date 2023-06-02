namespace Commerce.Security.Utils;

public class TokenResponse
{
    public string? AccessToken { get; set; }
    public string Code { get; set; } = "Error";
    public string? Message { get; set; }
}