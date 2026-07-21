using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.CustomerFolder
{
    public class CustomerTests
    {
        // Helper per istanziare oggetti validi rapidamente nei test
        private Name GetValidName() => Name.Create("Mario").Value;
        private Surname GetValidSurname() => Surname.Create("Rossi").Value;
        private Email GetValidEmail() => Email.Create("mario.rossi@example.com").Value;
        private Address GetValidAddress() => Address.Create("Via Roma", "10", "Milano", "20100").Value;
        private PhoneNumber GetValidPhoneNumber() => PhoneNumber.Create("3331234567").Value;
        private TaxCode GetValidTaxCode() => TaxCode.Create("RSSMRA80A01H501U").Value;

        #region Customer Creation Tests

        [Fact]
        public void Customer_Create_WithValidData_ShouldSucceedAndRaiseEvent()
        {
            // Act
            var result = Customer.Create(
                GetValidName(),
                GetValidSurname(),
                GetValidEmail(),
                GetValidAddress(),
                null, // CreditCard opzionale
                GetValidPhoneNumber(),
                GetValidTaxCode()
            );

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Value.Should().Be("Mario");
            result.Value.Surname.Value.Should().Be("Rossi");
            result.Value.Email.Value.Should().Be("mario.rossi@example.com");

            // Verifica l'evento di dominio generato
            result.Value._domainEvents.Should().HaveCount(1);
        }

        #endregion

        #region Change Methods Tests

        [Fact]
        public void ChangeName_WithValidName_ShouldUpdateAndRaiseEvent()
        {
            // Arrange
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), null, GetValidPhoneNumber(), GetValidTaxCode()
            ).Value;

            var newName = Name.Create("Luigi").Value;

            // Act
            var result = customer.ChangeName(newName);

            // Assert
            result.IsSuccess.Should().BeTrue();
            customer.Name.Value.Should().Be("Luigi");
            customer._domainEvents.Should().HaveCount(2); // 1 Creazione + 1 Cambio Nome
        }

        [Fact]
        public void ChangeName_WithNullValue_ShouldFail()
        {
            // Arrange
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), null, GetValidPhoneNumber(), GetValidTaxCode()
            ).Value;

            // Act
            var result = customer.ChangeName(null!);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Change name");
        }

        [Fact]
        public void ChangeName_WithSameValue_ShouldDoNothingAndSucceed()
        {
            // Arrange
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), null, GetValidPhoneNumber(), GetValidTaxCode()
            ).Value;

            var sameName = GetValidName(); // "Mario"

            // Act
            var result = customer.ChangeName(sameName);

            // Assert
            result.IsSuccess.Should().BeTrue();
            customer._domainEvents.Should().HaveCount(1); // Nessun evento aggiuntivo generato
        }

        [Fact]
        public void ChangeEmail_WithValidEmail_ShouldUpdateAndRaiseEvent()
        {
            // Arrange
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), null, GetValidPhoneNumber(), GetValidTaxCode()
            ).Value;

            var newEmail = Email.Create("nuova.email@example.com").Value;

            // Act
            var result = customer.ChangeEmail(newEmail);

            // Assert
            result.IsSuccess.Should().BeTrue();
            customer.Email.Value.Should().Be("nuova.email@example.com");
        }

        #endregion

        #region Value Objects Unit Tests

        [Theory]
        [InlineData("2010")] // Meno di 5 cifre
        [InlineData("201000")] // Più di 5 cifre
        [InlineData(null)]
        [InlineData("")]
        public void Address_Create_WithInvalidCAP_ShouldFail(string invalidCap)
        {
            // Act
            var result = Address.Create("Via Roma", "10", "Milano", invalidCap);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Address CAP");
        }

        [Theory]
        [InlineData("RSSMRA80A01H50")] // Meno di 16 caratteri
        [InlineData("RSSMRA80A01H501UU")] // Più di 16 caratteri
        [InlineData("RSSMRA80A01H501!")] // Carattere speciale non valido
        public void TaxCode_Create_WithInvalidValue_ShouldFail(string invalidTaxCode)
        {
            // Act
            var result = TaxCode.Create(invalidTaxCode);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tax code creation");
        }

        [Theory]
        [InlineData("email-non-valida")]
        [InlineData("test@")]
        [InlineData("@domain.com")]
        public void Email_Create_WithInvalidFormat_ShouldFail(string invalidEmail)
        {
            // Act
            var result = Email.Create(invalidEmail);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Email");
        }

        #endregion

    }
}
