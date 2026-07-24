using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.CreditCardFolder
{
    public class CreditCardTests
    {
        #region CardOwner Tests

        [Fact]
        public void CardOwner_Create_WithValidData_ShouldSucceed()
        {
            var result = CardOwner.Create("Mario", "Rossi");

            result.IsSuccess.Should().BeTrue();
            result.Value.ToString().Should().Be("Mario Rossi");
        }

        [Theory]
        [InlineData("", "Rossi")]
        [InlineData(null, "Rossi")]
        [InlineData("Mario", "")]
        [InlineData("Mario", null)]
        public void CardOwner_Create_WithInvalidData_ShouldFail(string name, string surname)
        {
            var result = CardOwner.Create(name, surname);

            result.IsFailure.Should().BeTrue();
        }

        #endregion

        #region ExpiryDate Tests

        [Theory]
        [InlineData("05/26")]
        [InlineData("12/30")]
        [InlineData("01/25")]
        public void ExpiryDate_Create_WithValidFormat_ShouldSucceed(string rawDate)
        {
            var result = ExpiryDate.Create(rawDate);

            result.IsSuccess.Should().BeTrue();
            result.Value.Month.Should().BeGreaterThan(0).And.BeLessThanOrEqualTo(12);
        }

        [Theory]
        [InlineData("13/25")]   
        [InlineData("00/25")]  
        [InlineData("5/25")]  
        [InlineData("05/2025")] 
        [InlineData("invalid")]
        [InlineData(null)]
        public void ExpiryDate_Create_WithInvalidFormat_ShouldFail(string rawDate)
        {
            var result = ExpiryDate.Create(rawDate);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Expiry date");
        }

        [Fact]
        public void ExpiryDate_IsExpired_WhenReferenceDateIsAfter_ShouldReturnTrue()
        {
            var expiryDate = ExpiryDate.Create("05/24").Value;
            var referenceDate = new DateTime(2024, 6, 1); // Giugno 2024 (Scaduta a Maggio 2024)

            var isExpired = expiryDate.IsExpired(referenceDate);

            isExpired.Should().BeTrue();
        }

        #endregion

        #region CreditCard Entity Tests

        [Fact]
        public void CreditCard_Create_WithValidData_ShouldSucceed()
        {
            var result = AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.CreditCard.Create(
                "Mario",
                "Rossi",
                "12/28",
                "4321"
            );

            result.IsSuccess.Should().BeTrue();
            result.Value.DisplayName.Should().Be("XXXX-XXXX-XXXX-4321");
            result.Value.Last4Digits.Should().Be("4321");
        }

        [Theory]
        [InlineData("123")]  
        [InlineData("12345")] 
        [InlineData("")]     
        [InlineData(null)]
        public void CreditCard_Create_WithInvalidLast4Digits_ShouldFail(string invalidLast4Digits)
        {
            var result = AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.CreditCard.Create(
                "Mario",
                "Rossi",
                "12/28",
                invalidLast4Digits
            );

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Last 4 digits card");
        }
        #endregion

    }
}
