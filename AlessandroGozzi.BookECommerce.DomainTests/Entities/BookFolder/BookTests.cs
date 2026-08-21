using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Event;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.BookFolder
{
    public class BookTests
    {
        private static Subject ValidSubject => Subject.Create("Informatica").Value;
        private static ISBN ValidIsbn => ISBN.Create("978-0-306-40615-7").Value;
        private static ImageUrl ValidPhoto => ImageUrl.Create("book.jpg").Value;
        private static Money ValidPrice => Money.Create(19.99m).Value; // Assuming Money exists

        // ==========================================
        // CREATE TESTS
        // ==========================================
        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccess()
        {
            var sellerId = Guid.NewGuid();

            var result = Book.Create("Clean Code", sellerId, ValidSubject, ValidIsbn, 3, 2020, ValidPrice, BookStatus.LikeNew, ValidPhoto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Title.Should().Be("Clean Code");
            result.Value.SellerId.Should().Be(sellerId);
            result.Value.SchoolYear.Should().Be(3);
            result.Value.PublicationYear.Should().Be(2020);
            result.Value.IsAvailable.Should().BeTrue();
            result.Value.AverageRating.Should().Be(0);
            result.Value.RatingsNumber.Should().Be(0);
            result.Value.Reviews.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WhenTitleIsNullOrEmpty_ShouldReturnFailure(string? invalidTitle)
        {
            var result = Book.Create(invalidTitle!, Guid.NewGuid(), ValidSubject, ValidIsbn, 3, 2020, ValidPrice, BookStatus.LikeNew, ValidPhoto);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Title");
            result.Error.Description.Should().Be("Title cannot be null");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenSubjectIsNull_ShouldReturnFailure()
        {
            var result = Book.Create("Title", Guid.NewGuid(), null!, ValidIsbn, 3, 2020, ValidPrice, BookStatus.LikeNew, ValidPhoto);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Subject");
            result.Error.Description.Should().Be("Subject cannot be null");
        }

        [Fact]
        public void Create_WhenIsbnIsNull_ShouldReturnFailure()
        {
            var result = Book.Create("Title", Guid.NewGuid(), ValidSubject, null!, 3, 2020, ValidPrice, BookStatus.LikeNew, ValidPhoto);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("ISBN code");
            result.Error.Description.Should().Be("ISBN cannot be null");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(6)]
        public void Create_WhenSchoolYearIsInvalid_ShouldReturnFailure(int invalidSchoolYear)
        {
            var result = Book.Create("Title", Guid.NewGuid(), ValidSubject, ValidIsbn, invalidSchoolYear, 2020, ValidPrice, BookStatus.LikeNew, ValidPhoto);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("School book year");
            result.Error.Description.Should().Be("School year of book must be between 1 and 5");
        }

        [Theory]
        [InlineData(1999)]
        [InlineData(2030)]
        public void Create_WhenPublicationYearIsInvalid_ShouldReturnFailure(int invalidPubYear)
        {
            var result = Book.Create("Title", Guid.NewGuid(), ValidSubject, ValidIsbn, 3, invalidPubYear, ValidPrice, BookStatus.LikeNew, ValidPhoto);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Publication book year");
            result.Error.Description.Should().Be("Book publication must be between 2000 and current year");
        }

        [Fact]
        public void Create_WhenPriceIsNull_ShouldReturnFailure()
        {
            var result = Book.Create("Title", Guid.NewGuid(), ValidSubject, ValidIsbn, 3, 2020, null!, BookStatus.LikeNew, ValidPhoto);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Price");
            result.Error.Description.Should().Be("Book price can't be null");
        }

        [Fact]
        public void Create_WhenPhotoIsNull_ShouldReturnFailure()
        {
            var result = Book.Create("Title", Guid.NewGuid(), ValidSubject, ValidIsbn, 3, 2020, ValidPrice, BookStatus.LikeNew, null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Photo");
            result.Error.Description.Should().Be("Book photo can't be null");
        }

        // ==========================================
        // REVIEWS TESTS
        // ==========================================
        [Fact]
        public void AddReview_WithValidData_ShouldUpdateRatingsAndAddReview()
        {
            var book = CreateValidBook();
            var review = BookReview.Create(Guid.NewGuid(), new FullName(Name.Create("Jane").Value, Surname.Create("Doe").Value), 4).Value;

            var result = book.AddReview(review);

            result.IsSuccess.Should().BeTrue();
            book.Reviews.Should().ContainSingle().Which.Should().Be(review);
            book.RatingsNumber.Should().Be(1);
            book.AverageRating.Should().Be(4.0);
        }

        [Fact]
        public void AddReview_WhenSellerTriesToReviewOwnBook_ShouldReturnSelfReviewError()
        {
            var sellerId = Guid.NewGuid();
            var book = CreateValidBook(sellerId);
            var review = BookReview.Create(sellerId, new FullName(Name.Create("Jane").Value, Surname.Create("Doe").Value), 5).Value;

            var result = book.AddReview(review);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Review.SelfReview");
            result.Error.Description.Should().Be("Book seller cannot add self review");
            result.Error.Type.Should().Be(ErrorType.StatusConflict);
        }

        [Fact]
        public void AddReview_WhenCustomerAlreadyReviewed_ShouldReturnDuplicateError()
        {
            var book = CreateValidBook();
            var customerId = Guid.NewGuid();
            var review1 = BookReview.Create(customerId, new FullName(Name.Create("Jane").Value, Surname.Create("Doe").Value), 4).Value;
            var review2 = BookReview.Create(customerId, new FullName(Name.Create("Jane").Value, Surname.Create("Doe").Value), 5).Value;

            book.AddReview(review1);
            var result = book.AddReview(review2);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Review.Duplicate");
            result.Error.Description.Should().Be("This customer has already reviewed this book.");
            result.Error.Type.Should().Be(ErrorType.StatusConflict);
        }

        [Fact]
        public void RemoveReview_WhenReviewExists_ShouldRecalculateAverageAndRemove()
        {
            var book = CreateValidBook();
            var customerId = Guid.NewGuid();
            var review = BookReview.Create(customerId, new FullName(Name.Create("Jane").Value, Surname.Create("Doe").Value), 4).Value;
            book.AddReview(review);

            var result = book.RemoveReview(customerId);

            result.IsSuccess.Should().BeTrue();
            book.Reviews.Should().BeEmpty();
            book.RatingsNumber.Should().Be(0);
            book.AverageRating.Should().Be(0.0);
        }

        [Fact]
        public void RemoveReview_WhenReviewDoesNotExist_ShouldReturnNotFoundError()
        {
            var book = CreateValidBook();

            var result = book.RemoveReview(Guid.NewGuid());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Review");
            result.Error.Description.Should().Be("No review found for this customer on this book.");
            result.Error.Type.Should().Be(ErrorType.NotFound);
        }

        // ==========================================
        // CHANGE PHOTO TESTS
        // ==========================================
        [Fact]
        public void ChangePhoto_WhenRequesterIsSeller_ShouldUpdatePhoto()
        {
            var sellerId = Guid.NewGuid();
            var book = CreateValidBook(sellerId);
            var newPhoto = ImageUrl.Create("new_cover.png").Value;

            var result = book.ChangePhoto(sellerId, newPhoto);

            result.IsSuccess.Should().BeTrue();
            book.MainPhoto.Should().Be(newPhoto);
        }

        [Fact]
        public void ChangePhoto_WhenRequesterIsNotSeller_ShouldReturnPermissionDenied()
        {
            var book = CreateValidBook();
            var newPhoto = ImageUrl.Create("new_cover.png").Value;

            var result = book.ChangePhoto(Guid.NewGuid(), newPhoto);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("New photo");
            result.Error.Description.Should().Be("Cannot update photo. You are not the seller");
            result.Error.Type.Should().Be(ErrorType.PermissionDenied);
        }

        [Fact]
        public void ChangePhoto_WhenPhotoIsNull_ShouldReturnValidationError()
        {
            var sellerId = Guid.NewGuid();
            var book = CreateValidBook(sellerId);

            var result = book.ChangePhoto(sellerId, null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("New photo");
            result.Error.Description.Should().Be("Cannot update null photo");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        // ==========================================
        // UPDATE PRICE TESTS
        // ==========================================
        [Fact]
        public void UpdatePrice_WhenRequesterIsSeller_ShouldUpdatePrice()
        {
            var sellerId = Guid.NewGuid();
            var book = CreateValidBook(sellerId);
            var newPrice = Money.Create(25.00m).Value;

            var result = book.UpdatePrice(newPrice, sellerId);

            result.IsSuccess.Should().BeTrue();
            book.Price.Should().Be(newPrice);
        }

        [Fact]
        public void UpdatePrice_WhenPriceIsSame_ShouldReturnSuccessWithoutError()
        {
            var sellerId = Guid.NewGuid();
            var book = CreateValidBook(sellerId);

            var result = book.UpdatePrice(ValidPrice, sellerId);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void UpdatePrice_WhenRequesterIsNotSeller_ShouldReturnPermissionDenied()
        {
            var book = CreateValidBook();
            var newPrice = Money.Create(25.00m).Value;

            var result = book.UpdatePrice(newPrice, Guid.NewGuid());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("New price");
            result.Error.Type.Should().Be(ErrorType.PermissionDenied);
        }

        [Fact]
        public void UpdatePrice_WhenPriceIsNull_ShouldReturnValidationError()
        {
            var sellerId = Guid.NewGuid();
            var book = CreateValidBook(sellerId);

            var result = book.UpdatePrice(null!, sellerId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("New price");
            result.Error.Description.Should().Be("Cannot update to a null price");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        // ==========================================
        // UPDATE STATUS TESTS
        // ==========================================
        [Fact]
        public void UpdateStatus_WhenRequesterIsSeller_ShouldUpdateStatus()
        {
            var sellerId = Guid.NewGuid();
            var book = CreateValidBook(sellerId);

            var result = book.UpdateStatus(BookStatus.LikeNew, sellerId);

            result.IsSuccess.Should().BeTrue();
            book.Status.Should().Be(BookStatus.LikeNew);
        }

        [Fact]
        public void UpdateStatus_WhenRequesterIsNotSeller_ShouldReturnPermissionDenied()
        {
            var book = CreateValidBook();

            var result = book.UpdateStatus(BookStatus.LikeNew, Guid.NewGuid());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("New status");
            result.Error.Type.Should().Be(ErrorType.PermissionDenied);
        }

        // ==========================================
        // MARKET AVAILABILITY TESTS
        // ==========================================
        [Fact]
        public void RemoveFromMarket_WhenAvailableAndSeller_ShouldSetIsAvailableToFalse()
        {
            var sellerId = Guid.NewGuid();
            var book = CreateValidBook(sellerId);

            var result = book.RemoveFromMarket(sellerId);

            result.IsSuccess.Should().BeTrue();
            book.IsAvailable.Should().BeFalse();
        }

        [Fact]
        public void RemoveFromMarket_WhenAlreadyRemoved_ShouldReturnStatusConflict()
        {
            var sellerId = Guid.NewGuid();
            var book = CreateValidBook(sellerId);
            book.RemoveFromMarket(sellerId);

            var result = book.RemoveFromMarket(sellerId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book.Remove");
            result.Error.Description.Should().Be("Book is already removed from market");
            result.Error.Type.Should().Be(ErrorType.StatusConflict);
        }

        [Fact]
        public void RestoreInMarket_WhenNotAvailableAndSeller_ShouldSetIsAvailableToTrue()
        {
            var sellerId = Guid.NewGuid();
            var book = CreateValidBook(sellerId);
            book.RemoveFromMarket(sellerId);

            var result = book.RestoreInMarket(sellerId);

            result.IsSuccess.Should().BeTrue();
            book.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public void RestoreInMarket_WhenAlreadyAvailable_ShouldReturnStatusConflict()
        {
            var sellerId = Guid.NewGuid();
            var book = CreateValidBook(sellerId);

            var result = book.RestoreInMarket(sellerId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book.Restore");
            result.Error.Description.Should().Be("Book is already restored in market");
            result.Error.Type.Should().Be(ErrorType.StatusConflict);
        }

        // Helper method
        private static Book CreateValidBook(Guid? sellerId = null)
        {
            return Book.Create(
                "Test Book",
                sellerId ?? Guid.NewGuid(),
                ValidSubject,
                ValidIsbn,
                3,
                2020,
                ValidPrice,
                BookStatus.LikeNew,
                ValidPhoto
            ).Value;
        }
    }
}
