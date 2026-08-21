using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Event;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.CustomerFolder
{
    public class CustomerTests
    {
        private readonly Name _validName = Name.Create("Mario").Value;
        private readonly Surname _validSurname = Surname.Create("Rossi").Value;
        private readonly Email _validEmail = Email.Create("mario.rossi@example.com").Value;
        private readonly Address _validAddress = Address.Create("Via Roma", "10", "Milano", "20100").Value;
        private readonly PhoneNumber _validPhone = PhoneNumber.Create("3401234567").Value;
        private readonly TaxCode _validTaxCode = TaxCode.Create("RSSMRA80A01H501Z").Value;
        private const string ValidPasswordHash = "hashed_password_123";

        private Customer CreateSampleCustomer()
        {
            return Customer.Create(
                _validName,
                _validSurname,
                _validEmail,
                _validAddress,
                _validPhone,
                _validTaxCode,
                ValidPasswordHash).Value;
        }

        #region Create Tests

        [Fact]
        public void Create_ShouldSucceed_AndRaiseCustomerCreatedEvent_WhenParametersAreValid()
        {
            var result = Customer.Create(
                GetValidName(),
                GetValidSurname(),
                GetValidEmail(),
                GetValidAddress(),
                GetValidPhoneNumber(),
                GetValidTaxCode(),
                "hashedPassword123"
            );

            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be(_validName);
            result.Value._domainEvents.Should().ContainSingle(e => e is CustomerCreatedEvent);
        }

        [Fact]
        public void Create_ShouldFail_WhenNameIsNull()
        {
            var result = Customer.Create(
                null!,
                _validSurname,
                _validEmail,
                _validAddress,
                _validPhone,
                _validTaxCode,
                ValidPasswordHash);

            result.IsFailure.Should().BeTrue();
        }

        #endregion

        #region ChangeName Tests

        [Fact]
        public void ChangeName_ShouldFail_WhenNameIsNull()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode(), "hashedPassword123"
            ).Value;

            var result = customer.ChangeName(null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Change name");
        }

        [Fact]
        public void ChangeName_ShouldDoNothing_WhenNameIsUnchanged()
        {
            var customer = CreateSampleCustomer();
            customer._domainEvents.Clear();

            var result = customer.ChangeName(_validName);

            result.IsSuccess.Should().BeTrue();
            customer._domainEvents.Should().BeEmpty();
        }

        [Fact]
        public void ChangeName_ShouldUpdateNameAndRaiseEvent_WhenNameIsNew()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode(), "hashedPassword123"
            ).Value;

            var result = customer.ChangeName(newName);

            result.IsSuccess.Should().BeTrue();
            customer.Name.Should().Be(newName);
            customer._domainEvents.Should().ContainSingle(e => e is NameChangedEvent);
        }

        #endregion

        #region ChangeSurname Tests

        [Fact]
        public void ChangeSurname_ShouldUpdateSurnameAndRaiseEvent_WhenSurnameIsNew()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode(), "hashedPassword123"
            ).Value;

            var result = customer.ChangeSurname(newSurname);

            result.IsSuccess.Should().BeTrue();
            customer.Surname.Should().Be(newSurname);
            customer._domainEvents.Should().ContainSingle(e => e is SurnameChangedEvent);
        }

        #endregion

        #region ChangeEmail Tests

        [Fact]
        public void ChangeEmail_ShouldUpdateEmailAndRaiseEvent_WhenEmailIsNew()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode(), "hashedPassword123"
            ).Value;

            var newEmail = Email.Create("nuova.email@example.com").Value;

            var result = customer.ChangeEmail(newEmail);

            result.IsSuccess.Should().BeTrue();
            customer.Email.Should().Be(newEmail);
            customer._domainEvents.Should().ContainSingle(e => e is EmailChangedEvent);
        }

        #endregion

        #region ChangeAddress Tests

        [Fact]
        public void ChangeAddress_ShouldUpdateAddressAndRaiseEvent_WhenAddressIsNew()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode(), "hashedPassword123"
            ).Value;
            CreditCard nullCard = null;

            var result = customer.ChangeAddress(newAddress);

            result.IsSuccess.Should().BeTrue();
            customer.Address.Should().Be(newAddress);
            customer._domainEvents.Should().ContainSingle(e => e is AddressChangedEvent);
        }

        #endregion

        #region AddCreditCard Tests

        [Fact]
        public void AddCreditCard_ShouldFail_WhenCardIsNull()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode(), "hashedPassword123"
            ).Value;
            var card = CreditCard.Create("Alessandro", "Gozzi", "10/30", "0672").Value;

            customer.AddCreditCard(card);
            customer.ClearEvents(); 

            var result = customer.AddCreditCard(null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Credit card");
        }

        [Fact]
        public void AddCreditCard_ShouldSetCardAndRaiseEvent_WhenCardIsValid()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode(), "hashedPassword123"
            ).Value;

            var card = CreditCard.Create("Alessandro", "Gozzi", "10/30", "0672").Value;
            customer.AddCreditCard(card);
            customer.ClearEvents();

            var card2 = CreditCard.Create("Noemi", "Colciago", "10/31", "0670").Value;

            var result = customer.AddCreditCard(card);

            result.IsSuccess.Should().BeTrue();
            customer.CreditCard.Should().Be(card);
            customer._domainEvents.Should().ContainSingle(e => e is CreditCardAddedEvent);
        }

        #endregion

        #region ChangePassword Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ChangePassword_ShouldFail_WhenPasswordHashIsInvalid(string? invalidHash)
        {
            var customer = CreateSampleCustomer();

            var result = customer.ChangePassword(invalidHash!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("New password");
        }

        [Fact]
        public void ChangePassword_ShouldUpdatePasswordAndRaiseEvent_WhenPasswordIsNew()
        {
            var customer = CreateSampleCustomer();
            customer._domainEvents.Clear();
            const string newHash = "new_secret_hash_999";

            var result = customer.ChangePassword(newHash);

            result.IsSuccess.Should().BeTrue();
            customer.PasswordHash.Should().Be(newHash);
            customer._domainEvents.Should().ContainSingle(e => e is CustomerPasswordChangedEvent);
        }

        #endregion

    }
}
