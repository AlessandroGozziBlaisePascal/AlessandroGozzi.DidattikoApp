using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.ValueObject
{
    public class ValueObjectTests
    {
        #region ISBN Tests

        public class IsbnTests
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void Create_ShouldFail_WhenValueIsEmpty(string? invalidValue)
            {
                var result = ISBN.Create(invalidValue!);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("ISBN.Empty");
            }

            [Theory]
            [InlineData("123")]
            [InlineData("12345678901234")]
            [InlineData("978-3-16-148410-X")]
            [InlineData("INVALID_ISBN")]
            public void Create_ShouldFail_WhenFormatIsInvalid(string invalidFormat)
            {
                var result = ISBN.Create(invalidFormat);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("ISBN.InvalidFormat");
            }

            [Theory]
            [InlineData("978-0-306-40615-7")]
            [InlineData("9780306406157")]   
            [InlineData("0-306-40615-2")] 
            [InlineData("0306406152")]       
            [InlineData("0-8044-2957-X")]   
            public void Create_ShouldSucceed_WhenIsbnIsValid(string validIsbn)
            {
                var result = ISBN.Create(validIsbn);

                result.IsSuccess.Should().BeTrue();
                result.Value.Should().NotBeNull();
                result.Value.Value.Should().NotBeNullOrWhiteSpace();
            }

            [Theory]
            [InlineData("978-0-306-40615-8")]
            [InlineData("0-306-40615-9")]   
            public void Create_ShouldFail_WhenCheckDigitIsInvalid(string invalidCheckDigitIsbn)
            {
                var result = ISBN.Create(invalidCheckDigitIsbn);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("ISBN.InvalidCheckDigit");
            }

        }
        #endregion

        #region Subject Tests

        public class SubjectTests
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void Create_ShouldFail_WhenValueIsEmpty(string? invalidValue)
            {
                var result = Subject.Create(invalidValue!);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Subject empty");
            }

            [Theory]
            [InlineData("Science!")]
            [InlineData("Math_101")]
            [InlineData("History#")]
            [InlineData("Books & Novel")]
            public void Create_ShouldFail_WhenContainsSpecialCharacters(string invalidValue)
            {
                var result = Subject.Create(invalidValue);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Subject");
            }

            [Theory]
            [InlineData("Informatica")]
            [InlineData("Math 101")]
            [InlineData("Storia Contemporanea 2026")]
            public void Create_ShouldSucceed_WhenValueIsValidAlphanumeric(string validValue)
            {
                var result = Subject.Create(validValue);

                result.IsSuccess.Should().BeTrue();
                result.Value.Value.Should().Be(validValue);
            }
        }

        #endregion

        #region CartItem Tests

        public class CartItemTests
        {
            private readonly Guid _validBookId = Guid.NewGuid();
            private readonly string _validTitle = "Il Nome della Rosa";
            private readonly Money _validPrice = Money.Create(19.99m).Value;
            private readonly string _validPhoto = "https://example.com/cover.jpg";
            private readonly int _validQuantity = 2;

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void Create_ShouldFail_WhenTitleIsEmpty(string? invalidTitle)
            {
                var result = CartItem.Create(_validBookId, invalidTitle!, _validPrice, _validPhoto, _validQuantity);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Cart item title");
            }

            [Fact]
            public void Create_ShouldFail_WhenPriceIsNull()
            {
                var result = CartItem.Create(_validBookId, _validTitle, null!, _validPhoto, _validQuantity);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Cart item price");
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void Create_ShouldFail_WhenPhotoIsEmpty(string? invalidPhoto)
            {
                var result = CartItem.Create(_validBookId, _validTitle, _validPrice, invalidPhoto!, _validQuantity);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Cart item main photo");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(-10)]
            public void Create_ShouldFail_WhenQuantityIsLessThanOne(int invalidQuantity)
            {
                var result = CartItem.Create(_validBookId, _validTitle, _validPrice, _validPhoto, invalidQuantity);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Cart item quantity");
            }

            [Fact]
            public void Create_ShouldSucceed_WhenAllParametersAreValid()
            {
                var result = CartItem.Create(_validBookId, _validTitle, _validPrice, _validPhoto, _validQuantity);

                result.IsSuccess.Should().BeTrue();
                result.Value.Should().NotBeNull();
                result.Value.BookId.Should().Be(_validBookId);
                result.Value.BookTitle.Should().Be(_validTitle);
                result.Value.Price.Should().Be(_validPrice);
                result.Value.MainPhoto.Should().Be(_validPhoto);
                result.Value.Quantity.Should().Be(_validQuantity);
            }

            [Fact]
            public void UpdateQuantity_ShouldUpdateQuantityProperty()
            {
                var item = CartItem.Create(_validBookId, _validTitle, _validPrice, _validPhoto, 1).Value;

                item.UpdateQuantity(4);

                item.Quantity.Should().Be(5);
            }
        }

        #endregion
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

        #region Address Tests

        [Theory]
        [InlineData(null, "10", "Roma", "00100")]
        [InlineData("", "10", "Roma", "00100")]
        [InlineData("   ", "10", "Roma", "00100")]
        public void Address_Create_ShouldFail_WhenStreetIsInvalid(string? street, string civic, string city, string cap)
        {
            var result = Address.Create(street!, civic, city, cap);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Address street");
        }

        [Theory]
        [InlineData("Via Roma", null, "Roma", "00100")]
        [InlineData("Via Roma", "", "Roma", "00100")]
        public void Address_Create_ShouldFail_WhenCivicNumberIsInvalid(string street, string? civic, string city, string cap)
        {
            var result = Address.Create(street, civic!, city, cap);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Address civic number");
        }

        [Theory]
        [InlineData("Via Roma", "10", null, "00100")]
        [InlineData("Via Roma", "10", "", "00100")]
        public void Address_Create_ShouldFail_WhenCityIsInvalid(string street, string civic, string? city, string cap)
        {
            var result = Address.Create(street, civic, city!, cap);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Address city");
        }

        [Theory]
        [InlineData("Via Roma", "10", "Roma", null)]
        [InlineData("Via Roma", "10", "Roma", "1234")]
        [InlineData("Via Roma", "10", "Roma", "123456")]
        [InlineData("Via Roma", "10", "Roma", "ABCDE")]
        public void Address_Create_ShouldFail_WhenCAPIsInvalid(string street, string civic, string city, string? cap)
        {
            var result = Address.Create(street, civic, city, cap!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Address CAP");
        }

        [Fact]
        public void Address_Create_ShouldSucceed_WhenValid()
        {
            var result = Address.Create("Via Roma", "12/A", "Milano", "20100");

            result.IsSuccess.Should().BeTrue();
            result.Value.ToString().Should().Be("Via Roma 12/A, Milano 20100");
        }

        #endregion

        #region Email Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Email_Create_ShouldFail_WhenEmpty(string? mail)
        {
            var result = Email.Create(mail!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Email creation");
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("test@")]
        [InlineData("@domain.com")]
        public void Email_Create_ShouldFail_WhenFormatIsInvalid(string mail)
        {
            var result = Email.Create(mail);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Email");
        }

        [Fact]
        public void Email_Create_ShouldSucceed_WhenValid()
        {
            var result = Email.Create(" test@example.com ");

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("test@example.com");
        }

        #endregion

        #region Name Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Name_Create_ShouldFail_WhenEmpty(string? name)
        {
            var result = Name.Create(name!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Name creation");
        }

        [Theory]
        [InlineData("Mario123")]
        [InlineData("Mario!")]
        public void Name_Create_ShouldFail_WhenContainsNumbersOrSpecialChars(string name)
        {
            var result = Name.Create(name);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Name creation");
        }

        [Fact]
        public void Name_Create_ShouldSucceed_WhenValid()
        {
            var result = Name.Create(" Jean-Luc ");

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("Jean-Luc");
        }

        #endregion

        #region Surname Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Surname_Create_ShouldFail_WhenEmpty(string? surname)
        {
            var result = Surname.Create(surname!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Surname creation");
        }

        [Theory]
        [InlineData("Rossi99")]
        [InlineData("Rossi#")]
        public void Surname_Create_ShouldFail_WhenInvalid(string surname)
        {
            var result = Surname.Create(surname);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Surname creation");
        }

        [Fact]
        public void Surname_Create_ShouldSucceed_WhenValid()
        {
            var result = Surname.Create("D'Angelo");

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("D'Angelo");
        }

        #endregion

        #region PhoneNumber Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void PhoneNumber_Create_ShouldFail_WhenEmpty(string? phone)
        {
            var result = PhoneNumber.Create(phone!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Phone number creation");
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("12345678901234")]
        public void PhoneNumber_Create_ShouldFail_WhenLengthOutOfRange(string phone)
        {
            var result = PhoneNumber.Create(phone);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Phone number creation");
        }

        [Fact]
        public void PhoneNumber_Create_ShouldSucceed_AndCleanPrefixes()
        {
            var result = PhoneNumber.Create(" +39 340 123 4567 ");

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("3401234567");
            result.Value.InternationalNumber.Should().Be("+393401234567");
        }

        #endregion

        #region TaxCode Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void TaxCode_Create_ShouldFail_WhenEmpty(string? code)
        {
            var result = TaxCode.Create(code!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tax code creation");
        }

        [Theory]
        [InlineData("RSSMRA80A01H501")]
        [InlineData("RSSMRA80A01H501111")]
        public void TaxCode_Create_ShouldFail_WhenLengthIsNot16(string code)
        {
            var result = TaxCode.Create(code);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tax code creation");
        }

        [Fact]
        public void TaxCode_Create_ShouldFail_WhenContainsSpecialCharacters()
        {
            var result = TaxCode.Create("RSSMRA80A01H501!");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tax code creation");
        }

        [Fact]
        public void TaxCode_Create_ShouldSucceed_WhenValid()
        {
            var result = TaxCode.Create(" rssmra80a01h501z ");

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be("RSSMRA80A01H501Z");
        }

        #endregion
        #region Money Tests

        [Fact]
        public void Money_Create_ShouldFail_WhenAmountIsNegative()
        {
            var result = Money.Create(-10m);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Money amount");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(19.99)]
        public void Money_Create_ShouldSucceed_WhenAmountIsZeroOrPositive(decimal amount)
        {
            var result = Money.Create(amount);

            result.IsSuccess.Should().BeTrue();
            result.Value.Amount.Should().Be(amount);
        }

        [Fact]
        public void Money_AdditionOperator_ShouldSumAmounts()
        {
            var m1 = Money.Create(10m).Value;
            var m2 = Money.Create(15m).Value;

            var total = m1 + m2;

            total.Amount.Should().Be(25m);
        }

        [Fact]
        public void Money_MultiplyOperator_ShouldMultiplyAmount()
        {
            var m1 = Money.Create(10m).Value;

            var total = m1 * 3;

            total.Amount.Should().Be(30m);
        }

        #endregion

        #region OrderItem Tests

        [Fact]
        public void OrderItem_Create_ShouldFail_WhenBookIdIsEmpty()
        {
            var price = Money.Create(10m).Value;
            var result = OrderItem.Create(Guid.Empty, Guid.NewGuid(), "Il Nome della Rosa", price, 1);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("BookId");
        }

        [Fact]
        public void OrderItem_Create_ShouldFail_WhenSellerIdIsEmpty()
        {
            var price = Money.Create(10m).Value;
            var result = OrderItem.Create(Guid.NewGuid(), Guid.Empty, "Il Nome della Rosa", price, 1);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("SellerId");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void OrderItem_Create_ShouldFail_WhenTitleIsInvalid(string? title)
        {
            var price = Money.Create(10m).Value;
            var result = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), title!, price, 1);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("BookTitle");
        }

        [Fact]
        public void OrderItem_Create_ShouldFail_WhenPriceIsNull()
        {
            var result = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Il Nome della Rosa", null!, 1);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("BookPrice");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void OrderItem_Create_ShouldFail_WhenQuantityIsLessThanOne(int quantity)
        {
            var price = Money.Create(10m).Value;
            var result = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Il Nome della Rosa", price, quantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Quantity");
        }

        [Fact]
        public void OrderItem_Create_ShouldSucceed_AndCalculateTotalPriceCorrectly()
        {
            var price = Money.Create(15m).Value;
            var result = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Il Nome della Rosa", price, 3);

            result.IsSuccess.Should().BeTrue();
            result.Value.TotalPrice.Amount.Should().Be(45m);
        }

        #endregion

    }
}