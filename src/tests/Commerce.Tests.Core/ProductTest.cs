using Commerce.Core.Entities;
using FluentAssertions;
using FluentValidation;

namespace Commerce.Tests.Core
{
    public class ProductTest
    {
        private string name = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        [Fact]
        public void Name_CreateNewUser_shouldThrowOverflowMaxCharacters() //what?_how?_outcome
        {
            //arrange

            //act
            Action res = () =>
            {
                Product product = new Product(name, "a", 1, "a");
            };
            //assert
            res.Should().Throw<ValidationException>().WithMessage("O nome do produto n�o deve ultrapassar 50 caracteres.");
        }
    }
}