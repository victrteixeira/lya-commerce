using System.Security.Cryptography;
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
    public string FirstName { get; }
    public string LastName { get; }
    public string EmailAddress { get; }
    public string HashedPassword { get; }
    
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