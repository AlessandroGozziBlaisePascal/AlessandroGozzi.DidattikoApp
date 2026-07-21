using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using AlessandroGozzi_BookECommerce.Domain.Entities;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Event;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.BookFolder
{
    public class BookTests
    {
        private readonly Guid _sellerId = Guid.NewGuid();
        private readonly Subject _validSubject = Subject.Create("Informatica").Value;
        private readonly ISBN _validIsbn = ISBN.Create("978-88-08-09703-3").Value; 
        private readonly Money _validPrice = Money.Create(29.99m).Value;

        private Book CreateSampleBook()
        {
            return Book.Create(
                "Algoritmi e Strutture Dati",
                _sellerId,
                _validSubject,
                _validIsbn,
                3,
                2021,
                _validPrice,
                BookStatus.LikeNew 
            ).Value;
        }

        #region Factory Method (Create) Tests

        [Fact]
        public void Create_WithValidData_ShouldCreateBookAndBeAvailable()
        {
            var bookResult = Book.Create("Analisi Matematica", _sellerId, _validSubject, _validIsbn, 1, 2022, _validPrice, BookStatus.LikeNew);

            bookResult.IsSuccess.Should().BeTrue();
            var book = bookResult.Value;
            book.Title.Should().Be("Analisi Matematica");
            book.SellerId.Should().Be(_sellerId);
            book.IsAvailable.Should().BeTrue();
            book.AverageRating.Should().Be(0);
            book.RatingsNumber.Should().Be(0);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidTitle_ShouldReturnFailure(string invalidTitle)
        {
            var result = Book.Create(invalidTitle, _sellerId, _validSubject, _validIsbn, 1, 2022, _validPrice, BookStatus.LikeNew);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Title");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void Create_WithInvalidSchoolYear_ShouldReturnFailure(int invalidSchoolYear)
        {
            var result = Book.Create("Fisica", _sellerId, _validSubject, _validIsbn, invalidSchoolYear, 2022, _validPrice, BookStatus.LikeNew);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("School book year");
        }

        #endregion

        #region Reviews Tests & Average Rating Calculation

        [Fact]
        public void AddReview_WithValidReview_ShouldUpdateRatingsAndRaiseEvent()
        {
            var book = CreateSampleBook();
            var reviewerId = Guid.NewGuid();
            var review = BookReview.Create(reviewerId, 4, "Molto chiaro").Value;

            var result = book.AddReview(review);

            result.IsSuccess.Should().BeTrue();
            book.Reviews.Should().HaveCount(1);
            book.RatingsNumber.Should().Be(1);
            book.AverageRating.Should().Be(4.0);

            book._domainEvents.Should().ContainSingle(e => e is ReviewAddedEvent);
        }

        [Fact]
        public void AddReview_DuplicateReviewFromSameCustomer_ShouldReturnFailure()
        {
            var book = CreateSampleBook();
            var reviewerId = Guid.NewGuid();
            var review1 = BookReview.Create(reviewerId, 5, "Ottimo").Value;
            var review2 = BookReview.Create(reviewerId, 3, "In realtà mediocre").Value;

            book.AddReview(review1);

            var result = book.AddReview(review2);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Review.Duplicate");
            book.Reviews.Should().HaveCount(1);
        }

        [Fact]
        public void AddReview_SelfReviewBySeller_ShouldReturnFailure()
        {
            var book = CreateSampleBook();
            var selfReview = BookReview.Create(_sellerId, 5, "Mio libro bellissimo").Value;

            var result = book.AddReview(selfReview);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Review.SelfReview");
        }

        [Fact]
        public void RemoveReview_ExistingReview_ShouldRecalculateAverageRatingAndRaiseEvent()
        {
            var book = CreateSampleBook();
            var customer1 = Guid.NewGuid();
            var customer2 = Guid.NewGuid();

            book.AddReview(BookReview.Create(customer1, 5, "Eccellente").Value);
            book.AddReview(BookReview.Create(customer2, 1, "Pessimo").Value);

            var result = book.RemoveReview(customer2);

            result.IsSuccess.Should().BeTrue();
            book.Reviews.Should().HaveCount(1);
            book.RatingsNumber.Should().Be(1);
            book.AverageRating.Should().Be(5.0);

            book._domainEvents.Should().Contain(e => e is ReviewRemovedEvent);
        }

        #endregion

        #region Update Price & Status Tests

        [Fact]
        public void UpdatePrice_BySeller_ShouldUpdatePriceAndRaiseEvent()
        {
            var book = CreateSampleBook();
            var newPrice = Money.Create(19.99m).Value;

            var result = book.UpdatePrice(newPrice, _sellerId);

            result.IsSuccess.Should().BeTrue();
            book.Price.Should().Be(newPrice);
            book._domainEvents.Should().ContainSingle(e => e is PriceUpdatedEvent);
        }

        [Fact]
        public void UpdatePrice_ByNotSeller_ShouldReturnPermissionDenied()
        {
            var book = CreateSampleBook();
            var wrongCustomer = Guid.NewGuid();
            var newPrice = Money.Create(19.99m).Value;

            var result = book.UpdatePrice(newPrice, wrongCustomer);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("New price");
        }

        #endregion

        #region RemoveFromMarket Tests

        [Fact]
        public void RemoveFromMarket_WhenAvailable_ShouldSetIsAvailableToFalseAndRaiseEvent()
        {
            var book = CreateSampleBook();

            var result = book.RemoveFromMarket();

            result.IsSuccess.Should().BeTrue();
            book.IsAvailable.Should().BeFalse();
            book._domainEvents.Should().ContainSingle(e => e is BookRemovedFromMarketEvent);
        }

        [Fact]
        public void RemoveFromMarket_WhenAlreadyRemoved_ShouldReturnFailure()
        {
            var book = CreateSampleBook();
            book.RemoveFromMarket(); // Prima rimozione

            var result = book.RemoveFromMarket(); // Seconda rimozione

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book.Remove");
        }

        #endregion

    }
}
