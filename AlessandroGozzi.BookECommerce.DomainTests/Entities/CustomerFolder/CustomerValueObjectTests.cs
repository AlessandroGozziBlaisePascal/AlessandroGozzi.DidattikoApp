using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.CustomerFolder
{
    public class CustomerValueObjectsTests
    {
        #region ADDRESS TESTS
        [Fact]
        public void Address_Create_WithValidParameters_ShouldReturnSuccess()
        {
            var result = Address.Create("Via Roma", "10", "Milano", "20100");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Street.Should().Be("Via Roma");
            result.Value.CivicNumber.Should().Be("10");
            result.Value.City.Should().Be("Milano");
            result.Value.CAP.Should().Be("20100");
            result.Value.ToString().Should().Be("Via Roma 10, Milano 20100");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Address_Create_WhenStreetIsNullOrEmpty_ShouldReturnFailure(string? invalidStreet)
        {
            var result = Address.Create(invalidStreet!, "10", "Milano", "20100");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Address street");
            result.Error.Description.Should().Be("Street cannot be null or empty");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Address_Create_WhenCivicNumberIsNullOrEmpty_ShouldReturnFailure(string? invalidCivic)
        {
            var result = Address.Create("Via Roma", invalidCivic!, "Milano", "20100");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Address civic number");
            result.Error.Description.Should().Be("Civic number cannot be null or empty");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Address_Create_WhenCityIsNullOrEmpty_ShouldReturnFailure(string? invalidCity)
        {
            var result = Address.Create("Via Roma", "10", invalidCity!, "20100");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Address city");
            result.Error.Description.Should().Be("City cannot be null or empty");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("2010")] // Meno di 5 cifre
        [InlineData("201000")] // Più di 5 cifre
        [InlineData("2010A")] // Contiene lettere
        public void Address_Create_WhenCapIsInvalid_ShouldReturnFailure(string? invalidCap)
        {
            var result = Address.Create("Via Roma", "10", "Milano", invalidCap!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Address CAP");
            result.Error.Description.Should().Be("CAP must be exactly 5 numeric digits");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }
        #endregion

        #region EMAIL TESTS
        [Theory]
        [InlineData("mario.rossi@example.com", "mario.rossi@example.com")]
        [InlineData("  test@domain.it  ", "test@domain.it")]
        public void Email_Create_WithValidEmail_ShouldReturnSuccessAndTrim(string rawEmail, string expectedEmail)
        {
            var result = Email.Create(rawEmail);

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(expectedEmail);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Email_Create_WhenNullOrWhiteSpace_ShouldReturnFailure(string? invalidEmail)
        {
            var result = Email.Create(invalidEmail!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Email creation");
            result.Error.Description.Should().Be("Email cannot be null or white spaces");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("mario@")]
        [InlineData("@domain.com")]
        public void Email_Create_WhenFormatIsInvalid_ShouldReturnFailure(string invalidEmail)
        {
            var result = Email.Create(invalidEmail);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Email");
            result.Error.Description.Should().Be("Email incorrect format");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }
        #endregion
        #region NAME TESTS
        [Theory]
        [InlineData("Mario", "Mario")]
        [InlineData("  Maria  Giulia ", "Maria Giulia")]
        [InlineData("Jean-Luc", "Jean-Luc")]
        public void Name_Create_WithValidValue_ShouldReturnSuccess(string rawName, string expectedName)
        {
            var result = Name.Create(rawName);

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(expectedName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Name_Create_WhenNullOrWhiteSpace_ShouldReturnFailure(string? invalidName)
        {
            var result = Name.Create(invalidName!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Name creation");
            result.Error.Description.Should().Be("Name cannot be null or white spaces");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Name_Create_WhenMoreThan3Names_ShouldReturnFailure()
        {
            var result = Name.Create("Mario Giuseppe Antonio Luca");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Name creation");
            result.Error.Description.Should().Be("You can have max 3 names");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData("Mario123")]
        [InlineData("Mario!")]
        public void Name_Create_WhenContainsNonLetters_ShouldReturnFailure(string invalidName)
        {
            var result = Name.Create(invalidName);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Name creation");
            result.Error.Description.Should().Be("Name must contain only letters");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }
        #endregion
        #region SURNAME TESTS
        [Theory]
        [InlineData("Rossi", "Rossi")]
        [InlineData(" De  Sica ", "De Sica")]
        public void Surname_Create_WithValidValue_ShouldReturnSuccess(string rawSurname, string expectedSurname)
        {
            var result = Surname.Create(rawSurname);

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(expectedSurname);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Surname_Create_WhenNullOrWhiteSpace_ShouldReturnFailure(string? invalidSurname)
        {
            var result = Surname.Create(invalidSurname!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Surname creation");
            result.Error.Description.Should().Be("Surname cannot be null ro white spaces");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Surname_Create_WhenMoreThan4Surnames_ShouldReturnFailure()
        {
            var result = Surname.Create("Rossi Bianchi Neri Verde Blu");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Surname creation");
            result.Error.Description.Should().Be("You can have max 4 surnames");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData("Rossi123")]
        [InlineData("Rossi@")]
        public void Surname_Create_WhenContainsNonLetters_ShouldReturnFailure(string invalidSurname)
        {
            var result = Surname.Create(invalidSurname);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Surname creation");
            result.Error.Description.Should().Be("Surname must contain only letters");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }
        #endregion
        #region FULLNAME TESTS
        [Fact]
        public void FullName_ToString_ShouldFormatCorrectly()
        {
            var name = Name.Create("Mario").Value;
            var surname = Surname.Create("Rossi").Value;
            var fullName = new FullName(name, surname);

            fullName.ToString().Should().Be("Mario Rossi");
        }
        #endregion
        #region PHONE NUMBER TESTS
        [Theory]
        [InlineData("3331234567", "3331234567")]
        [InlineData(" +39 333 123 4567 ", "3331234567")]
        [InlineData("00393331234567", "3331234567")]
        [InlineData("333-123-4567", "3331234567")]
        public void PhoneNumber_Create_WithValidFormat_ShouldCleanAndReturnSuccess(string rawPhone, string expectedValue)
        {
            var result = PhoneNumber.Create(rawPhone);

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(expectedValue);
            result.Value.InternationalNumber.Should().Be($"+39{expectedValue}");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void PhoneNumber_Create_WhenNullOrWhiteSpace_ShouldReturnFailure(string? invalidPhone)
        {
            var result = PhoneNumber.Create(invalidPhone!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Phone number creation");
            result.Error.Description.Should().Be("Number cannot be null or white spaces");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData("12345678")] // 8 cifre (meno di 9)
        [InlineData("123456789012")] // 12 cifre (più di 11)
        public void PhoneNumber_Create_WhenLengthIsInvalid_ShouldReturnFailure(string invalidPhone)
        {
            var result = PhoneNumber.Create(invalidPhone);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Phone number creation");
            result.Error.Description.Should().Be("Phone number must be between 9 and 11 digits");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void PhoneNumber_Create_WhenContainsNonDigits_ShouldReturnFailure()
        {
            var result = PhoneNumber.Create("33312345a7");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Phone number creation");
            result.Error.Description.Should().Be("Phone must be composed of only digits");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }
        #endregion
        #region TAX CODE TESTS
        [Theory]
        [InlineData("RSSMRA80A01H501U", "RSSMRA80A01H501U")]
        [InlineData("  rssmra80a01h501u ", "RSSMRA80A01H501U")]
        public void TaxCode_Create_WithValidValue_ShouldUpperAndReturnSuccess(string rawTaxCode, string expectedTaxCode)
        {
            var result = TaxCode.Create(rawTaxCode);

            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(expectedTaxCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void TaxCode_Create_WhenNullOrWhiteSpace_ShouldReturnFailure(string? invalidTaxCode)
        {
            var result = TaxCode.Create(invalidTaxCode!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tax code creation");
            result.Error.Description.Should().Be("Tax code cannot be null or white spaces");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData("RSSMRA80A01H501")] // 15 caratteri
        [InlineData("RSSMRA80A01H501XX")] // 17 caratteri
        public void TaxCode_Create_WhenLengthIsNot16_ShouldReturnFailure(string invalidTaxCode)
        {
            var result = TaxCode.Create(invalidTaxCode);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tax code creation");
            result.Error.Description.Should().Be("Tax code lenght must be 16 digits");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void TaxCode_Create_WhenContainsSpecialCharacters_ShouldReturnFailure()
        {
            var result = TaxCode.Create("RSSMRA80A01H501!");

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tax code creation");
            result.Error.Description.Should().Be("Tax code must be comped by letters or digits");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }
        #endregion
    }
}
