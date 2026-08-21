using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.BookFolder
{
    public class BookValueObjectTests
    {
        #region BOOK REVIEW TESTS
        public class BookReviewTests
        {
            [Fact]
            public void Create_WithValidData_ShouldReturnSuccess()
            {
                var customerId = Guid.NewGuid();
                var customerName = new FullName(Name.Create("John").Value, Surname.Create("Doe").Value);
                int rating = 4;

                var result = BookReview.Create(customerId, customerName, rating);

                result.IsSuccess.Should().BeTrue();
                result.Value.Should().NotBeNull();
                result.Value.CustomerId.Should().Be(customerId);
                result.Value.CustomerName.Should().Be(customerName);
                result.Value.Rating.Should().Be(rating);
                result.Value.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            }

            [Fact]
            public void Create_WhenCustomerNameIsNull_ShouldReturnFailure()
            {
                var result = BookReview.Create(Guid.NewGuid(), null!, 5);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Customer name");
                result.Error.Description.Should().Be("Customer name is null");
                result.Error.Type.Should().Be(ErrorType.Validation);
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(6)]
            public void Create_WhenRatingIsOutOfRange_ShouldReturnFailure(int invalidRating)
            {
                var customerName = new FullName(Name.Create("John").Value, Surname.Create("Doe").Value);

                var result = BookReview.Create(Guid.NewGuid(), customerName, invalidRating);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Review.InvalidRating");
                result.Error.Description.Should().Be("The rating must be between 1 and 5 stars.");
                result.Error.Type.Should().Be(ErrorType.Validation);
            }

            [Fact]
            public void ToString_ShouldReturnFormattedString()
            {
                var customerName = new FullName(Name.Create("John").Value, Surname.Create("Doe").Value);
                var review = BookReview.Create(Guid.NewGuid(), customerName, 5).Value;

                var result = review.ToString();

                result.Should().Be($"{review.CustomerName} {review.Rating} {review.CreatedAt}");
            }
        }
        #endregion
        #region IMAGE URL TESTS
        public class ImageUrlTests
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void Create_WhenNullOrWhitespace_ShouldReturnDefaultImage(string? input)
            {
                var result = ImageUrl.Create(input);

                result.IsSuccess.Should().BeTrue();
                result.Value.Value.Should().Be("default-book.jpg");
            }

            [Theory]
            [InlineData("cover.jpg")]
            [InlineData("photo.JPEG")]
            [InlineData("image.png")]
            [InlineData("pic.webp")]
            [InlineData("  space_trimmed.png  ")]
            public void Create_WithValidExtension_ShouldReturnSuccess(string input)
            {
                var result = ImageUrl.Create(input);

                result.IsSuccess.Should().BeTrue();
                result.Value.Value.Should().Be(input.Trim());
            }

            [Theory]
            [InlineData("file.pdf")]
            [InlineData("image.gif")]
            [InlineData("noextension")]
            public void Create_WithInvalidExtension_ShouldReturnFailure(string input)
            {
                var result = ImageUrl.Create(input);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("ImageUrl.InvalidFormat");
                result.Error.Description.Should().Be("L'immagine deve avere un'estensione valida (.jpg, .jpeg, .png, .webp).");
                result.Error.Type.Should().Be(ErrorType.Validation);
            }
        }
        #endregion
        #region ISBN TESTS
        public class IsbnTests
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void Create_WhenNullOrEmpty_ShouldReturnEmptyError(string? input)
            {
                var result = ISBN.Create(input!);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("ISBN.Empty");
                result.Error.Description.Should().Be("ISBN cannot be empty.");
            }

            [Theory]
            [InlineData("12345")]
            [InlineData("INVALID_ISBN_TEXT")]
            [InlineData("123456789012345")]
            public void Create_WithInvalidRegexFormat_ShouldReturnFormatError(string input)
            {
                var result = ISBN.Create(input);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("ISBN.InvalidFormat");
            }

            [Theory]
            [InlineData("978-0-306-40615-7", "9780306406157")]
            [InlineData("0-306-40615-2", "0306406152")]
            [InlineData("080442957X", "080442957X")]
            public void Create_WithValidIsbn_ShouldCleanAndReturnSuccess(string input, string expectedClean)
            {
                var result = ISBN.Create(input);

                result.IsSuccess.Should().BeTrue();
                result.Value.Value.Should().Be(expectedClean);
            }

            [Theory]
            [InlineData("9780306406158")]
            [InlineData("0306406153")]
            public void Create_WithInvalidCheckDigit_ShouldReturnCheckDigitError(string input)
            {
                var result = ISBN.Create(input);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("ISBN.InvalidCheckDigit");
                result.Error.Description.Should().Be("The provided ISBN code is mathematically invalid.");
            }
        }
        #endregion
        #region SUBJECT TESTS
        public class SubjectTests
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void Create_WhenNullOrEmpty_ShouldReturnEmptyError(string? input)
            {
                var result = Subject.Create(input!);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Subject empty");
                result.Error.Description.Should().Be("Subject cannot be null");
            }

            [Theory]
            [InlineData("Fantasy")]
            [InlineData("Saggistica italiana")]
            public void Create_WithValidString_ShouldReturnSuccess(string input)
            {
                var result = Subject.Create(input);

                result.IsSuccess.Should().BeTrue();
                result.Value.Value.Should().Be(input);
            }

            [Theory]
            [InlineData("Fantasy!")]
            [InlineData("Sci_Fi")]
            [InlineData("Books & Novel")]
            public void Create_WithSpecialCharacters_ShouldReturnRegexError(string input)
            {
                var result = Subject.Create(input);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Subject");
                result.Error.Description.Should().Be("Subject can only contains letters, digits or spaces");
            }
            #endregion
        }
    }
}