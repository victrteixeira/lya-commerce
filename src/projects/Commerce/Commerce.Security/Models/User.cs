using Commerce.Security.Validators;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Commerce.Security.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; private set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string Role { get; set; } = "User";
    public string HashedPassword { get; set; }
    
    public User(string firstname, string lastname, string emailAddress, string hashedPassword)
    {
        FirstName = firstname;
        LastName = lastname;
        EmailAddress = emailAddress;
        HashedPassword = hashedPassword;
        ValidateUser();
    }

    private void ValidateUser() => new UserValidator().ValidateAndThrow(this);
}