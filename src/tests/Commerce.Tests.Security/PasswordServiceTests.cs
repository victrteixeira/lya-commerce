using Commerce.Security.Helpers.Exceptions;
using Commerce.Security.Models;
using Commerce.Security.Services;
using FluentAssertions;
using ValidationException = FluentValidation.ValidationException;

namespace Commerce.Tests.Security;

public class PasswordServiceTests
{
    private readonly PasswordService _pwdService;

    public PasswordServiceTests() => _pwdService = new PasswordService();

    [Fact]
    public void HashPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        var pwd = "TestPassword@123";
        
        // Act
        var result = _pwdService.HashPassword(pwd);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().MatchRegex("^[a-fA-F0-9]+:[a-fA-F0-9]+:[0-9]+:[a-zA-Z0-9]+$");
    }

    [Fact]
    public void HashPassword_ShouldReturnNonNullString()
    {
        // Arrange
        var password = "TestPassword@123";
        
        // Act
        var result = _pwdService.HashPassword(password);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<string>();
    }

    [Fact]
    public void VerifyHash_ShouldReturnTrue_WhenPasswordMatchesHash()
    {
        // Arrange
        var password = "TestPassword@123";
        var hashed = _pwdService.HashPassword(password);

        // Act
        var result = _pwdService.ValidateHash(password, hashed);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyHash_ShouldReturnFalse_WhenPasswordDoesntMatchHash()
    {
        // Arrange
        var password = "TestPassword@123";
        var wrongPassword = "TestPwd";
        var hash = _pwdService.HashPassword(password);
        
        // Act
        var result = _pwdService.ValidateHash(wrongPassword, hash);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UpdatePasswordAsync_UpdatesPassword_WhenEverythingIsCorrect()
    {
        // Arrange
        var oldPwd = "oldPassword@123";
        var newPwd = "newPassword@123";
        
        var oldPwdHash = _pwdService.HashPassword(oldPwd);
        var user = new User("sample", "testing", "sampletest@testing.com", oldPwdHash);

        var oldResultHash = _pwdService.ValidateHash(oldPwd, oldPwdHash);
        
        // Act
        var updatedUser = _pwdService.UpdateHash(oldPwd, user, newPwd);
        var result = _pwdService.ValidateHash(newPwd, updatedUser.HashedPassword);
        
        // Assert
        Assert.True(oldResultHash && result);
    }

    [Fact]
    public void UpdatePasswordAsync_ShouldThrowException_WhenTheOldPasswordIsIncorrect()
    {
        var oldPwd = "oldPassword@123";
        var oldHash = _pwdService.HashPassword(oldPwd);
        var user = new User("sample", "testing", "sampletest@testing.com", oldHash);
        
        // Act
        Action res = () =>
        {
            var _ = _pwdService.UpdateHash("somethingElse", user, "SomethingNewToPwd123@");
        };
        
        // Assert
        res.Should()
            .Throw<InvalidPasswordException>()
            .WithMessage("A senha antiga está incorreta.");
    }

    [Fact]
    public void UpdatePasswordAsync_ShouldThrowException_WhenThePasswordDoesntPassValidations()
    {
        var oldPwd = "oldPassword@123";
        var oldHash = _pwdService.HashPassword(oldPwd);
        var user = new User("sample", "testing", "sampletest@testing.com", oldHash);
        
        // Act
        Action res = () =>
        {
            var _ = _pwdService.UpdateHash(oldPwd, user, "invalidpwd");
        };
        
        // Assert
        res.Should()
            .Throw<InvalidPasswordException>()
            .WithMessage("A senha precisa conter ao menos uma letra maiúscula.");
    }
}