namespace Commerce.Security.DTOs;

public class ReadUser
{
    public ReadUser(string? firstName, string? lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}