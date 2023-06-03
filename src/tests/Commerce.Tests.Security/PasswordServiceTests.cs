using Commerce.Security.Services;
using FluentAssertions;

namespace Commerce.Tests.Security;

public class PasswordServiceTests
{
    private readonly PasswordService _pwdService;

    public PasswordServiceTests()
    {
        _pwdService = new PasswordService();
    }

    [Fact]
    public async Task EncryptPassword_ShouldReturnHashedPassword()
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
    public async Task EncryptPassword_ShouldreturnNonNullString()
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
}