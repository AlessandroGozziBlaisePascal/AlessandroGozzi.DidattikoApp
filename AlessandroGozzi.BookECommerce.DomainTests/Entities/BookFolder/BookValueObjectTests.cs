using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Value_Object;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.BookFolder
{
    public class BookValueObjectTests
    {
        #region Subject Tests

        [Fact]
        public void Subject_Create_WithValidValue_ShouldSucceed()
        {
            // Act
            var result = Subject.Create("Matematica 101");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("Matematica 101");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Subject_Create_WithNullOrEmpty_ShouldReturnFailure(string invalidSubject)
        {
            // Act
            var result = Subject.Create(invalidSubject);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Subject empty");
        }

        #endregion

        #region ISBN Tests

        [Theory]
        [InlineData("978-88-08-09703-3")] // Sostituire con ISBN-13 valido
        [InlineData("9788808097033")] // Sostituire con ISBN-13 valido
        [InlineData("978-0-13-449416-6")]       // Sostituire con ISBN-10 valido
        [InlineData("0-13-609181-4")]
        [InlineData("007462542X")]
        public void ISBN_Create_WithValidIsbn_ShouldSucceed(string validIsbn)
        {
            // Act
            var result = ISBN.Create(validIsbn);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ISBN_Create_WithNullOrEmpty_ShouldReturnFailure(string invalidIsbn)
        {
            // Act
            var result = ISBN.Create(invalidIsbn);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("ISBN.Empty");
        }

        [Fact]
        public void ISBN_Create_WithInvalidChecksum_ShouldReturnFailure()
        {
            // Act (ISBN13 con cifre errate)
            var result = ISBN.Create("9781234567890");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("ISBN.InvalidCheckDigit");
        }

        #endregion

        #region BookReview Tests

        [Fact]
        public void BookReview_Create_WithValidData_ShouldSucceed()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            // Act
            var result = BookReview.Create(customerId, 5, "Ottimo libro di testo!");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.CustomerId.Should().Be(customerId);
            result.Value.Rating.Should().Be(5);
            result.Value.Text.Should().Be("Ottimo libro di testo!");
        }

        [Fact]
        public void BookReview_Create_WithEmptyCustomerId_ShouldReturnFailure()
        {
            // Act
            var result = BookReview.Create(Guid.Empty, 4, "Molto utile");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Review.InvalidCustomer");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void BookReview_Create_WithInvalidRating_ShouldReturnFailure(int invalidRating)
        {
            // Act
            var result = BookReview.Create(Guid.NewGuid(), invalidRating, "Bel libro");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Review.InvalidRating");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void BookReview_Create_WithEmptyText_ShouldReturnFailure(string invalidText)
        {
            // Act
            var result = BookReview.Create(Guid.NewGuid(), 5, invalidText);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Review.EmptyText");
        }

        #endregion

    }
}
