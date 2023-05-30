using Commerce.Security.Models;
using FluentAssertions;
using FluentValidation;

namespace Commerce.Tests.Security;

public class UserModelTests
{
    #region FirstName
    [Fact]
    public void FirstName_ShouldThrowAnException_NullName()
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User(null!, "something", "victor@victor.com", "sajkdsajkla");
        };
        // assert
        res.Should()
            .Throw<ValidationException>();
    }

    [Fact]
    public void FirstName_ShouldThrownAnException_EmptyName()
    {
        Action res = () =>
        {
            var _ = new User("", "something", "victor@victor.com", "sajkdsajkla");
        };
        // assert
        res.Should()
            .Throw<ValidationException>();
    }

    [Fact]
    public void FirstName_ShouldThrowAnException_MoreThan20Chars()
    {
        Action res = () =>
        {
            var _ = new User("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "something", "victor@victor.com", "sajkdsajkla");
        };
        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Fact]
    public void FirstName_ShouldThrowAnException_EqualToLastName()
    {
        Action res = () =>
        {
            var _ = new User("teixeira", "teixeira", "victor@victor.com", "sajkdsajkla");
        };
        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Fact]
    public void FirstName_ShouldThrowAnException_SpecialChars()
    {
        Action res = () =>
        {
            var _ = new User("v!ctor", "teixeira", "victor@victor.com", "sajkdsajkla");
        };
        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    #endregion

    #region LastName

    [Fact]
    public void LastName_ShouldThrowAnException_NullName()
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User("something", null!, "victor@victor.com", "sajkdsajkla");
        };
        // assert
        res.Should()
            .Throw<ValidationException>();
    }

    [Fact]
    public void LastName_ShouldThrownAnException_EmptyName()
    {
        Action res = () =>
        {
            var _ = new User("something", "", "victor@victor.com", "sajkdsajkla");
        };
        // assert
        res.Should()
            .Throw<ValidationException>();
    }

    [Fact]
    public void LastName_ShouldThrowAnException_MoreThan20Chars()
    {
        Action res = () =>
        {
            var _ = new User("something", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "victor@victor.com", "sajkdsajkla");
        };
        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Fact]
    public void LastName_ShouldThrowAnException_SpecialChars()
    {
        Action res = () =>
        {
            var _ = new User("victor", "teixeir@", "victor@victor.com", "sajkdsajkla");
        };
        // assert
        res.Should()
            .Throw<ValidationException>();
    }

    #endregion

    #region Email

    [Fact]
    [Trait("Category", "Email")]
    public void Email_ShouldThrowAnException_MissingAtSymbol()
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User("something", "something2", "userexample.com", "sajkdsajkla");
        };
        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Theory]
    [InlineData("u@ser@example.com")]
    [InlineData("user@exampl@e.com")]
    [Trait("Category", "Email")]
    public void Email_ShouldThrowAnException_TwoAtSymbols(string email)
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User("something", "something2", email, "sajkdsajkla");
        };
        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Fact]
    [Trait("Category", "Email")]
    public void Email_ShouldThrowAnException_MissingLocalPart()
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User("something", "something2", "user@", "sajkdsajkla");
        };
        
        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Fact]
    [Trait("Category", "Email")]
    public void Email_ShouldThrowAnException_MissingDomainPart()
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User("something", "something2", "@example.com", "sajkdsajkla");
        };

        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Fact]
    [Trait("Category", "Email")]
    public void Email_ShouldThrowAnException_SpecialChars()
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User("something", "something2", "u$er@example.com", "sajkdsajkla");
        };

        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Fact]
    [Trait("Category", "Email")]
    public void Email_ShouldThrowAnException_EmptyStringInTheMiddle()
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User("something", "something2", "user name@example.com", "sajkdsajkla");
        };

        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Fact]
    [Trait("Category", "Email")]
    public void Email_ShouldThrowAnException_ConsecutiveDots()
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User("something", "something2", "user..name@example.com", "sajkdsajkla");
        };

        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Fact]
    [Trait("Category", "Email")]
    public void Email_ShouldThrowAnException_BeginsWithDot()
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User("something", "something2", ".username@example.com", "sajkdsajkla");
        };

        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Fact]
    [Trait("Category", "Email")]
    public void Email_ShouldThrowAnException_WithoutDot()
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User("something", "something2", "username@examplecom", "sajkdsajkla");
        };

        // assert
        res.Should()
            .Throw<ValidationException>();
    }
    
    [Theory]
    [InlineData("username.model@example.com")]
    [InlineData("username.model777@example.com")]
    [InlineData("123usernamemodel@example.com")]
    [Trait("Category", "Email")]
    public void Email_ShouldNotThrowAnything_CorrectEmailModel(string email)
    {
        // arrange
        // act
        Action res = () =>
        {
            var _ = new User("something", "something2", email, "sajkdsajkla");
        };

        // assert
        res.Should()
            .NotThrow();
    }

    #endregion
    
    [Fact]
    public void UserModel_ShouldntThrowAnything_CorrectModel()
    {
        Action res = () =>
        {
            var _ = new User("victor", "teixeira", "victor@victor.com", "sajkdsajkla");
        };
        // assert
        res.Should()
            .NotThrow();
    }
}