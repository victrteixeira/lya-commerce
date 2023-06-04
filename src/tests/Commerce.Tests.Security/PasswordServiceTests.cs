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
    public async Task EncryptPasswordAsync_ShouldReturnHashedPassword()
    {
        // Arrange
        var pwd = "TestPassword";
        
        // Act
        var result = await _pwdService.EncryptPasswordAsync(pwd);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().MatchRegex("^[a-fA-F0-9]+:[a-fA-F0-9]+:[0-9]+:[a-zA-Z0-9]+$");
    }

    [Fact]
    public async Task EncryptPasswordAsync_ShouldReturnNonNullString()
    {
        // Arrange
        var password = "TestPassword";
        
        // Act
        var result = await _pwdService.EncryptPasswordAsync(password);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<string>();
    }

    [Fact]
    public async Task VerifyHash_ShouldReturnTrue_WhenPasswordMatchesHash()
    {
        // Arrange
        var password = "TestPassword";
        var hashed = await _pwdService.EncryptPasswordAsync(password);

        // Act
        var result = _pwdService.VerifyHash(password, hashed);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task VerifyHash_ShouldReturnFalse_WhenPasswordDoesntMatchHash()
    {
        // Arrange
        var password = "TestPassword";
        var wrongPassword = "TestPwd";
        var hash = await _pwdService.EncryptPasswordAsync(password);
        
        // Act
        var result = _pwdService.VerifyHash(wrongPassword, hash);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidatePassword_ShouldReturnTrue_WhenPasswordIsValid()
    {
        // Arrange
        var password = "Ttesting1";
        
        // Act
        var result = _pwdService.ValidatePassword(password);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData("Ttestin")] // Less than 8
    [InlineData("Ttesting")] // Without digit
    [InlineData("ttesting1")] // Without uppercase
    [InlineData("TTESTING1")] // Without lowercase
    public void ValidatePassword_ShouldReturnFalse_WhenPasswordIsntValid(string password)
    {
        // Arrange
        // Act
        var result = _pwdService.ValidatePassword(password);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdatePasswordAsync_UpdatesPassword_WhenEverythingIsCorrect()
    {
        // Arrange
        var oldPwd = "oldPassword@123";
        var newPwd = "newPassword@123";
        
        var oldPwdHash = await _pwdService.EncryptPasswordAsync(oldPwd);
        var user = new User("sample", "testing", "sampletest@testing.com", oldPwdHash);

        var oldResultHash = _pwdService.VerifyHash(oldPwd, oldPwdHash);
        
        // Act
        var updatedUser = await _pwdService.UpdatePasswordAsync(oldPwd, user, newPwd);
        var result = _pwdService.VerifyHash(newPwd, updatedUser.HashedPassword);
        
        // Assert
        Assert.True(oldResultHash && result);
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldThrowException_WhenTheOldPasswordIsIncorrect()
    {
        var oldPwd = "oldPassword@123";
        var oldHash = await _pwdService.EncryptPasswordAsync(oldPwd);
        var user = new User("sample", "testing", "sampletest@testing.com", oldHash);
        
        // Act
        Func<Task> res = async () =>
        {
            var _ = await _pwdService.UpdatePasswordAsync("somethingElse", user, "SomethingNewToPwd123@");
        };
        
        // Assert
        await res.Should()
            .ThrowAsync<InvalidPasswordException>()
            .WithMessage("A senha antiga está incorreta.");
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldThrowException_WhenThePasswordDoesntPassValidations()
    {
        var oldPwd = "oldPassword@123";
        var oldHash = await _pwdService.EncryptPasswordAsync(oldPwd);
        var user = new User("sample", "testing", "sampletest@testing.com", oldHash);
        
        // Act
        Func<Task> res = async () =>
        {
            var _ = await _pwdService.UpdatePasswordAsync(oldPwd, user, "invalidpwd");
        };
        
        // Assert
        await res.Should()
            .ThrowAsync<ValidationException>();
    }
}