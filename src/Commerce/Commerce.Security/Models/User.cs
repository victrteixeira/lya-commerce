﻿using Commerce.Security.Validators;

namespace Commerce.Security.Models;

public class User : Base
{
    public int Id { get; set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string EmailAddress { get; private set; }
    public string Password { get; private set; }
    public string HashedPassword { get; private set; }

    private void ValidateUser() => base.Validate(new UserValidator(), this);

    public User(string firstname, string lastname, string emailAddress, string password)
    {
        FirstName = firstname;
        LastName = lastname;
        EmailAddress = emailAddress;
        Password = password.Trim();
        ValidateUser();
        HashedPassword = EncryptPassword(password);
    }
}