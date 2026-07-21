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
            // Act
            var result = CardOwner.Create("Mario", "Rossi");

            // Assert
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
            // Act
            var result = CardOwner.Create(name, surname);

            // Assert
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
            // Act
            var result = ExpiryDate.Create(rawDate);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Month.Should().BeGreaterThan(0).And.BeLessThanOrEqualTo(12);
        }

        [Theory]
        [InlineData("13/25")]   // Mese invalido
        [InlineData("00/25")]   // Mese zero
        [InlineData("5/25")]    // Manca lo zero iniziale
        [InlineData("05/2025")] // Anno a 4 cifre anziché 2
        [InlineData("invalid")]
        [InlineData(null)]
        public void ExpiryDate_Create_WithInvalidFormat_ShouldFail(string rawDate)
        {
            // Act
            var result = ExpiryDate.Create(rawDate);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Expiry date");
        }

        [Fact]
        public void ExpiryDate_IsExpired_WhenReferenceDateIsAfter_ShouldReturnTrue()
        {
            // Arrange
            var expiryDate = ExpiryDate.Create("05/24").Value;
            var referenceDate = new DateTime(2024, 6, 1); // Giugno 2024 (Scaduta a Maggio 2024)

            // Act
            var isExpired = expiryDate.IsExpired(referenceDate);

            // Assert
            isExpired.Should().BeTrue();
        }

        #endregion

        #region CreditCard Entity Tests

        [Fact]
        public void CreditCard_Create_WithValidData_ShouldSucceed()
        {
            // Act
            var result = AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.CreditCard.Create(
                "Mario",
                "Rossi",
                "12/28",
                "4321",
                "tok_123456789"
            );

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.DisplayName.Should().Be("XXXX-XXXX-XXXX-4321");
            result.Value.Last4Digits.Should().Be("4321");
            result.Value.PaymentToken.Should().Be("tok_123456789");
        }

        [Theory]
        [InlineData("123")]   // Meno di 4 cifre
        [InlineData("12345")] // Più di 4 cifre
        [InlineData("")]      // Vuoto
        [InlineData(null)]
        public void CreditCard_Create_WithInvalidLast4Digits_ShouldFail(string invalidLast4Digits)
        {
            // Act
            var result = AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.CreditCard.Create(
                "Mario",
                "Rossi",
                "12/28",
                invalidLast4Digits,
                "tok_123456789"
            );

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Last 4 digits card");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreditCard_Create_WithEmptyToken_ShouldFail(string invalidToken)
        {
            // Act
            var result = AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.CreditCard.Create(
                "Mario",
                "Rossi",
                "12/28",
                "4321",
                invalidToken
            );

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Card token");
        }

        #endregion

    }
}
