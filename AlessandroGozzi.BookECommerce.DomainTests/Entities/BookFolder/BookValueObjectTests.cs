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
            var result = Subject.Create("Matematica 101");

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("Matematica 101");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Subject_Create_WithNullOrEmpty_ShouldReturnFailure(string invalidSubject)
        {
            var result = Subject.Create(invalidSubject);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Subject empty");
        }

        #endregion

        #region ISBN Tests

        [Theory]
        [InlineData("978-88-08-09703-3")] 
        [InlineData("9788808097033")] 
        [InlineData("978-0-13-449416-6")]     
        [InlineData("0-13-609181-4")]
        [InlineData("007462542X")]
        public void ISBN_Create_WithValidIsbn_ShouldSucceed(string validIsbn)
        {
            var result = ISBN.Create(validIsbn);

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ISBN_Create_WithNullOrEmpty_ShouldReturnFailure(string invalidIsbn)
        {
            var result = ISBN.Create(invalidIsbn);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("ISBN.Empty");
        }

        [Fact]
        public void ISBN_Create_WithInvalidChecksum_ShouldReturnFailure()
        {
            // ISBN13 con cifre errate
            var result = ISBN.Create("9781234567890");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("ISBN.InvalidCheckDigit");
        }

        #endregion

        #region BookReview Tests

        [Fact]
        public void BookReview_Create_WithValidData_ShouldSucceed()
        {
            var customerId = Guid.NewGuid();

            var result = BookReview.Create(customerId, 5, "Ottimo libro di testo!");

            result.IsSuccess.Should().BeTrue();
            result.Value.CustomerId.Should().Be(customerId);
            result.Value.Rating.Should().Be(5);
            result.Value.Text.Should().Be("Ottimo libro di testo!");
        }

        [Fact]
        public void BookReview_Create_WithEmptyCustomerId_ShouldReturnFailure()
        {
            var result = BookReview.Create(Guid.Empty, 4, "Molto utile");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Review.InvalidCustomer");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void BookReview_Create_WithInvalidRating_ShouldReturnFailure(int invalidRating)
        {
            var result = BookReview.Create(Guid.NewGuid(), invalidRating, "Bel libro");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Review.InvalidRating");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void BookReview_Create_WithEmptyText_ShouldReturnFailure(string invalidText)
        {
            var result = BookReview.Create(Guid.NewGuid(), 5, invalidText);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Review.EmptyText");
        }

        #endregion

    }
}
