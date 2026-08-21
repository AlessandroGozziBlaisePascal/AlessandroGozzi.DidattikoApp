using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.CreditCardFolder
{
    public class CreditCardValueObjectTests
    {
        // ==========================================
        // CREATE TESTS
        // ==========================================
        [Theory]
        [InlineData("12/28", 12, 2028)]
        [InlineData("01/25", 1, 2025)]
        [InlineData("09/30", 9, 2030)]
        public void Create_WithValidFormat_ShouldReturnSuccess(string rawDate, int expectedMonth, int expectedYear)
        {
            var result = ExpiryDate.Create(rawDate);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Month.Should().Be(expectedMonth);
            result.Value.Year.Should().Be(expectedYear);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("13/28")] // Mese non valido
        [InlineData("00/28")] // Mese non valido
        [InlineData("12/2028")] // Anno a 4 cifre invece di 2
        [InlineData("12-28")] // Separatore errato
        [InlineData("abc")]
        public void Create_WithInvalidFormat_ShouldReturnFailure(string? invalidDate)
        {
            var result = ExpiryDate.Create(invalidDate!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Expiry date");
            result.Error.Description.Should().Be("No valid format");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        // ==========================================
        // IS EXPIRED TESTS
        // ==========================================
        [Fact]
        public void IsExpired_WhenDateIsInThePast_ShouldReturnTrue()
        {
            var expiryDate = ExpiryDate.Create("05/24").Value;
            var referenceDate = new DateTime(2024, 06, 01);

            var isExpired = expiryDate.IsExpired(referenceDate);

            isExpired.Should().BeTrue();
        }

        [Fact]
        public void IsExpired_WhenDateIsCurrentMonth_ShouldReturnFalse()
        {
            var expiryDate = ExpiryDate.Create("05/24").Value;
            var referenceDate = new DateTime(2024, 05, 15);

            var isExpired = expiryDate.IsExpired(referenceDate);

            isExpired.Should().BeFalse();
        }

        [Fact]
        public void IsExpired_WhenDateIsInTheFuture_ShouldReturnFalse()
        {
            var expiryDate = ExpiryDate.Create("12/28").Value;
            var referenceDate = new DateTime(2024, 05, 15);

            var isExpired = expiryDate.IsExpired(referenceDate);

            isExpired.Should().BeFalse();
        }

        // ==========================================
        // TO STRING TESTS
        // ==========================================
        [Fact]
        public void ToString_ShouldReturnFormattedString()
        {
            var expiryDate = ExpiryDate.Create("08/26").Value;

            var result = expiryDate.ToString();

            result.Should().Be("8/2026");
        }
    }
}
