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
        private readonly Guid _validSellerId = Guid.NewGuid();
        private readonly Subject _validSubject = Subject.Create("Informatica").Value;
        private readonly ISBN _validIsbn = ISBN.Create("978-88-04-66823-7").Value;
        private readonly Money _validPrice = Money.Create(29.99m).Value;
        private readonly BookStatus _validStatus = BookStatus.PencilMarked; 

        private Book CreateValidBook()
        {

            var bookResult = Book.Create("TPSIT", _validSellerId, _validSubject, _validIsbn, 2, 2021, _validPrice, _validStatus);

            if (bookResult.IsFailure)
            {
                throw new InvalidOperationException($"Setup Book fallito: {bookResult.Error.Description}");
            }

            return bookResult.Value;
        }

        private BookReview CreateValidReview(Guid customerId, int rating = 5)
        {
            return BookReview.Create(customerId, rating, "Great book!").Value;
        }

        #region Create Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_ShouldFail_WhenTitleIsEmpty(string? invalidTitle)
        {
            var result = Book.Create(invalidTitle!, _validSellerId, _validSubject, _validIsbn, 3, 2021, _validPrice, _validStatus);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Create_ShouldFail_WhenSubjectIsNull()
        {
            var result = Book.Create("Title", _validSellerId, null!, _validIsbn, 3, 2021, _validPrice, _validStatus);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Create_ShouldFail_WhenIsbnIsNull()
        {
            var result = Book.Create("Title", _validSellerId, _validSubject, null!, 3, 2021, _validPrice, _validStatus);

            result.IsFailure.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-1)]
        public void Create_ShouldFail_WhenSchoolYearIsOutOfRange(int invalidSchoolYear)
        {
            var result = Book.Create("Title", _validSellerId, _validSubject, _validIsbn, invalidSchoolYear, 2021, _validPrice, _validStatus);

            result.IsFailure.Should().BeTrue();
        }

        [Theory]
        [InlineData(1999)]
        [InlineData(2030)]
        public void Create_ShouldFail_WhenPublicationYearIsInvalid(int invalidPubYear)
        {
            var result = Book.Create("Title", _validSellerId, _validSubject, _validIsbn, 3, invalidPubYear, _validPrice, _validStatus);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Create_ShouldFail_WhenPriceIsNull()
        {
            var result = Book.Create("Title", _validSellerId, _validSubject, _validIsbn, 3, 2021, null!, _validStatus);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Create_ShouldSucceed_WhenParametersAreValid()
        {
            var result = Book.Create("Clean Code", _validSellerId, _validSubject, _validIsbn, 3, 2021, _validPrice, _validStatus);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("Clean Code");
            result.Value.SellerId.Should().Be(_validSellerId);
            result.Value.Subject.Should().Be(_validSubject);
            result.Value.ISBNCode.Should().Be(_validIsbn);
            result.Value.SchoolYear.Should().Be(3);
            result.Value.PublicationYear.Should().Be(2021);
            result.Value.Price.Should().Be(_validPrice);
            result.Value.Status.Should().Be(_validStatus);
            result.Value.IsAvailable.Should().BeTrue();
            result.Value.MainPhoto.Should().Be("default_book_cover.png");
        }

        #endregion

        #region AddReview & RemoveReview Tests

        [Fact]
        public void AddReview_ShouldFail_WhenSellerTriesToReviewOwnBook()
        {
            var book = CreateValidBook();
            var review = CreateValidReview(_validSellerId);

            var result = book.AddReview(review);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void AddReview_ShouldFail_WhenCustomerHasAlreadyReviewed()
        {
            var book = CreateValidBook();
            var customerId = Guid.NewGuid();
            var review1 = CreateValidReview(customerId, 5);
            var review2 = CreateValidReview(customerId, 4);

            book.AddReview(review1);
            var result = book.AddReview(review2);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void AddReview_ShouldSucceedAndUpdateRatings_WhenReviewIsValid()
        {
            var book = CreateValidBook();
            var customerId = Guid.NewGuid();
            var review = CreateValidReview(customerId, 4);

            var result = book.AddReview(review);

            result.IsSuccess.Should().BeTrue();
            book.Reviews.Should().Contain(review);
            book.RatingsNumber.Should().Be(1);
            book.AverageRating.Should().Be(4.0);
        }

        [Fact]
        public void RemoveReview_ShouldFail_WhenReviewDoesNotExist()
        {
            var book = CreateValidBook();
            var nonExistentCustomerId = Guid.NewGuid();

            var result = book.RemoveReview(nonExistentCustomerId);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void RemoveReview_ShouldSucceedAndUpdateRatings_WhenReviewExists()
        {
            var book = CreateValidBook();
            var customerId = Guid.NewGuid();
            var review = CreateValidReview(customerId, 4);
            book.AddReview(review);

            var result = book.RemoveReview(customerId);

            result.IsSuccess.Should().BeTrue();
            book.Reviews.Should().NotContain(review);
            book.RatingsNumber.Should().Be(0);
            book.AverageRating.Should().Be(0.0);
        }

        #endregion

        #region AddPhotos Tests

        [Fact]
        public void AddPhotos_ShouldFail_WhenRequesterIsNotSeller()
        {
            var book = CreateValidBook();
            var strangerId = Guid.NewGuid();

            var result = book.AddPhotos(new List<string> { "photo1.jpg" }, strangerId);

            result.IsFailure.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        public void AddPhotos_ShouldFail_WhenPhotosListIsNull(List<string>? photos)
        {
            var book = CreateValidBook();

            var result = book.AddPhotos(photos!, _validSellerId);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void AddPhotos_ShouldFail_WhenPhotosListIsEmpty()
        {
            var book = CreateValidBook();

            var result = book.AddPhotos(new List<string>(), _validSellerId);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void AddPhotos_ShouldSucceed_WhenRequesterIsSellerAndPhotosAreValid()
        {
            var book = CreateValidBook();
            var photos = new List<string> { "cover.jpg", "back.jpg" };

            var result = book.AddPhotos(photos, _validSellerId);

            result.IsSuccess.Should().BeTrue();
            book.BookPhotos.Should().BeEquivalentTo(photos);
            book.MainPhoto.Should().Be("cover.jpg");
        }

        #endregion

        #region UpdatePrice Tests

        [Fact]
        public void UpdatePrice_ShouldFail_WhenRequesterIsNotSeller()
        {
            var book = CreateValidBook();
            var strangerId = Guid.NewGuid();
            var newPrice = Money.Create(15.00m).Value;

            var result = book.UpdatePrice(newPrice, strangerId);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void UpdatePrice_ShouldFail_WhenNewPriceIsNull()
        {
            var book = CreateValidBook();

            var result = book.UpdatePrice(null!, _validSellerId);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void UpdatePrice_ShouldSucceed_WhenPriceIsSame()
        {
            var book = CreateValidBook();

            var result = book.UpdatePrice(_validPrice, _validSellerId);

            result.IsSuccess.Should().BeTrue();
            book.Price.Should().Be(_validPrice);
        }

        [Fact]
        public void UpdatePrice_ShouldUpdatePrice_WhenValidNewPriceGiven()
        {
            var book = CreateValidBook();
            var newPrice = Money.Create(39.99m).Value;

            var result = book.UpdatePrice(newPrice, _validSellerId);

            result.IsSuccess.Should().BeTrue();
            book.Price.Should().Be(newPrice);
        }

        #endregion

        #region UpdateStatus Tests

        [Fact]
        public void UpdateStatus_ShouldFail_WhenRequesterIsNotSeller()
        {
            var book = CreateValidBook();
            var strangerId = Guid.NewGuid();

            var result = book.UpdateStatus(BookStatus.PenMarked, strangerId);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void UpdateStatus_ShouldSucceed_WhenStatusIsSame()
        {
            var book = CreateValidBook();

            var result = book.UpdateStatus(_validStatus, _validSellerId);

            result.IsSuccess.Should().BeTrue();
            book.Status.Should().Be(_validStatus);
        }

        [Fact]
        public void UpdateStatus_ShouldUpdateStatus_WhenValidNewStatusGiven()
        {
            var book = CreateValidBook();

            var result = book.UpdateStatus(BookStatus.PenMarked, _validSellerId);

            result.IsSuccess.Should().BeTrue();
            book.Status.Should().Be(BookStatus.PenMarked);
        }

        #endregion

        #region RemoveFromMarket & RestoreInMarket Tests

        [Fact]
        public void RemoveFromMarket_ShouldFail_WhenRequesterIsNotSeller()
        {
            var book = CreateValidBook();
            var strangerId = Guid.NewGuid();

            var result = book.RemoveFromMarket(strangerId);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void RemoveFromMarket_ShouldFail_WhenAlreadyRemoved()
        {
            var book = CreateValidBook();
            book.RemoveFromMarket(_validSellerId);

            var result = book.RemoveFromMarket(_validSellerId);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void RemoveFromMarket_ShouldSetIsAvailableToFalse()
        {
            var book = CreateValidBook();

            var result = book.RemoveFromMarket(_sellerId);

            result.IsSuccess.Should().BeTrue();
            book.IsAvailable.Should().BeFalse();
        }

        [Fact]
        public void RestoreInMarket_ShouldFail_WhenRequesterIsNotSeller()
        {
            var book = CreateValidBook();
            book.RemoveFromMarket(_validSellerId);
            var strangerId = Guid.NewGuid();

            var result = book.RestoreInMarket(strangerId);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void RestoreInMarket_ShouldFail_WhenAlreadyAvailable()
        {
            var book = CreateSampleBook();
            book.RemoveFromMarket(_sellerId); // Prima rimozione

            var result = book.RemoveFromMarket(_sellerId); // Seconda rimozione

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void RestoreInMarket_ShouldSetIsAvailableToTrue()
        {
            var book = CreateValidBook();
            book.RemoveFromMarket(_validSellerId);

            var result = book.RestoreInMarket(_validSellerId);

            result.IsSuccess.Should().BeTrue();
            book.IsAvailable.Should().BeTrue();
        } 

        #endregion

    }
}
