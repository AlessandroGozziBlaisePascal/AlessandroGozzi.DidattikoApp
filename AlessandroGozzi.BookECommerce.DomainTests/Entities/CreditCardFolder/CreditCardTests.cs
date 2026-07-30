using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.CreditCardFolder
{
    public class CreditCardTests
    {


        private const string ValidName = "Mario";
        private const string ValidSurname = "Rossi";
        private const string ValidExpiryDate = "12/28";
        private const string ValidLast4Digits = "1234";

        [Fact]
        public void Create_ShouldFail_WhenCardOwnerIsInvalid()
        {
            var result = CreditCard.Create("", ValidSurname, ValidExpiryDate, ValidLast4Digits);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Create_ShouldFail_WhenExpiryDateIsInvalid()
        {
            var result = CreditCard.Create(ValidName, ValidSurname, "13/99", ValidLast4Digits);

            result.IsFailure.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("123")]
        [InlineData("12345")]
        [InlineData("ABCD")]
        [InlineData("12A4")]
        public void Create_ShouldFail_WhenLast4DigitsAreInvalid(string? invalidDigits)
        {
            var result = CreditCard.Create(ValidName, ValidSurname, ValidExpiryDate, invalidDigits!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Last 4 digits card");
        }

        [Fact]
        public void Create_ShouldSucceed_WhenAllParametersAreValid()
        {
            var result = CreditCard.Create(ValidName, ValidSurname, ValidExpiryDate, ValidLast4Digits);

            result.IsSuccess.Should().BeTrue();
            result.Value.Last4Digits.Should().Be(ValidLast4Digits);
            result.Value.DisplayName.Should().Be("XXXX-XXXX-XXXX-1234");
        }

    }
}
