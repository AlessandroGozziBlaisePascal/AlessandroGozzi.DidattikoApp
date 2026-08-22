using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.CustomerEvent;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.CustomerFolder
{
    public class CustomerTests
    {
        private readonly FullName _fullName = new FullName(Name.Create("Alessandro").Value, Surname.Create("Gozzi").Value);
        private readonly Email _email = Email.Create("mario.rossi@example.com").Value;
        private readonly Address _address = Address.Create("Via Roma", "10", "Milano", "20100").Value;
        private readonly PhoneNumber _phoneNumber = PhoneNumber.Create("3331234567").Value;
        private readonly TaxCode _taxCode = TaxCode.Create("RSSMRA80A01H501U").Value;
        private const string PasswordHash = "hashed_password_123";

        private Customer CreateValidCustomer()
        {
            return Customer.Create(_fullName, _email, _address, _phoneNumber, _taxCode, PasswordHash).Value;
        }

        // ==========================================
        // CREATE TESTS
        // ==========================================
        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccessAndRaiseCustomerCreatedEvent()
        {
            var result = Customer.Create(_fullName, _email, _address, _phoneNumber, _taxCode, PasswordHash);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.FullName.Should().Be(_fullName);
            result.Value.Email.Should().Be(_email);
            result.Value.Address.Should().Be(_address);
            result.Value.Number.Should().Be(_phoneNumber);
            result.Value.TaxCode.Should().Be(_taxCode);
            result.Value.PasswordHash.Should().Be(PasswordHash);
            result.Value.Wallet.Should().NotBeNull();
            result.Value.Wallet.CustomerId.Should().Be(result.Value.Id);

            result.Value._domainEvents.Should().ContainSingle(e => e is CustomerCreatedEvent);
        }

        // ==========================================
        // CHANGE NAME TESTS
        // ==========================================
        [Fact]
        public void ChangeName_WhenNull_ShouldReturnFailure()
        {
            var customer = CreateValidCustomer();

            var result = customer.ChangeName(null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Change name");
            result.Error.Description.Should().Be("Cannot change to a null value");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void ChangeName_WhenSameValue_ShouldReturnSuccessWithoutDomainEvent()
        {
            var customer = CreateValidCustomer();
            customer._domainEvents.Clear();

            var result = customer.ChangeName(_fullName);

            result.IsSuccess.Should().BeTrue();
            customer._domainEvents.Should().BeEmpty();
        }

        [Fact]
        public void ChangeName_WithNewValue_ShouldUpdateAndRaiseNameChangedEvent()
        {
            var customer = CreateValidCustomer();
            customer._domainEvents.Clear();
            var newFullName = new FullName(Name.Create("Riccardo").Value, Surname.Create("Gozzi").Value);

            var result = customer.ChangeName(newFullName);

            result.IsSuccess.Should().BeTrue();
            customer.FullName.Should().Be(newFullName);
            customer._domainEvents.Should().ContainSingle(e => e is NameChangedEvent);
        }

        // ==========================================
        // CHANGE EMAIL TESTS
        // ==========================================
        [Fact]
        public void ChangeEmail_WhenNull_ShouldReturnFailure()
        {
            var customer = CreateValidCustomer();

            var result = customer.ChangeEmail(null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Change email");
            result.Error.Description.Should().Be("Cannot change to a null value");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void ChangeEmail_WhenSameValue_ShouldReturnSuccessWithoutDomainEvent()
        {
            var customer = CreateValidCustomer();
            customer._domainEvents.Clear();

            var result = customer.ChangeEmail(_email);

            result.IsSuccess.Should().BeTrue();
            customer._domainEvents.Should().BeEmpty();
        }

        [Fact]
        public void ChangeEmail_WithNewValue_ShouldUpdateAndRaiseEmailChangedEvent()
        {
            var customer = CreateValidCustomer();
            customer._domainEvents.Clear();
            var newEmail = Email.Create("luigi.rossi@example.com").Value;

            var result = customer.ChangeEmail(newEmail);

            result.IsSuccess.Should().BeTrue();
            customer.Email.Should().Be(newEmail);
            customer._domainEvents.Should().ContainSingle(e => e is EmailChangedEvent);
        }

        // ==========================================
        // CHANGE NUMBER TESTS
        // ==========================================
        [Fact]
        public void ChangeNumber_WhenNull_ShouldReturnFailure()
        {
            var customer = CreateValidCustomer();

            var result = customer.ChangeNumber(null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Change number");
            result.Error.Description.Should().Be("Cannot change to a null value");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void ChangeNumber_WhenSameValue_ShouldReturnSuccessWithoutDomainEvent()
        {
            var customer = CreateValidCustomer();
            customer._domainEvents.Clear();

            var result = customer.ChangeNumber(_phoneNumber);

            result.IsSuccess.Should().BeTrue();
            customer._domainEvents.Should().BeEmpty();
        }

        [Fact]
        public void ChangeNumber_WithNewValue_ShouldUpdateAndRaiseNumberChangedEvent()
        {
            var customer = CreateValidCustomer();
            customer._domainEvents.Clear();
            var newNumber = PhoneNumber.Create("3339876543").Value;

            var result = customer.ChangeNumber(newNumber);

            result.IsSuccess.Should().BeTrue();
            customer.Number.Should().Be(newNumber);
            customer._domainEvents.Should().ContainSingle(e => e is NumberChangedEvent);
        }

        // ==========================================
        // CHANGE ADDRESS TESTS
        // ==========================================
        [Fact]
        public void ChangeAddress_WhenNull_ShouldReturnFailure()
        {
            var customer = CreateValidCustomer();

            var result = customer.ChangeAddress(null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Change address");
            result.Error.Description.Should().Be("Cannot change to a null value");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void ChangeAddress_WhenSameValue_ShouldReturnSuccessWithoutDomainEvent()
        {
            var customer = CreateValidCustomer();
            customer._domainEvents.Clear();

            var result = customer.ChangeAddress(_address);

            result.IsSuccess.Should().BeTrue();
            customer._domainEvents.Should().BeEmpty();
        }

        [Fact]
        public void ChangeAddress_WithNewValue_ShouldUpdateAndRaiseAddressChangedEvent()
        {
            var customer = CreateValidCustomer();
            customer._domainEvents.Clear();
            var newAddress = Address.Create("Corso Vittorio", "20", "Torino", "10100").Value;

            var result = customer.ChangeAddress(newAddress);

            result.IsSuccess.Should().BeTrue();
            customer.Address.Should().Be(newAddress);
            customer._domainEvents.Should().ContainSingle(e => e is AddressChangedEvent);
        }

        // ==========================================
        // CREDIT CARD TESTS
        // ==========================================
        [Fact]
        public void AddCreditCard_WhenNull_ShouldReturnFailure()
        {
            var customer = CreateValidCustomer();

            var result = customer.AddCreditCard(null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Credit card");
            result.Error.Description.Should().Be("Cannot add null credit card");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void AddCreditCard_WithValidCard_ShouldUpdateAndRaiseCreditCardAddedEvent()
        {
            var customer = CreateValidCustomer();
            customer._domainEvents.Clear();
            var creditCard = CreditCard.Create("Mario", "Rossi", "08/30", "0654", customer.Id).Value;

            var result = customer.AddCreditCard(creditCard);

            result.IsSuccess.Should().BeTrue();
            customer.CreditCard.Should().Be(creditCard);
            customer._domainEvents.Should().ContainSingle(e => e is CreditCardAddedEvent);
        }

        [Fact]
        public void AddCreditCard_WhenSameCard_ShouldReturnSuccessWithoutDomainEvent()
        {
            var customer = CreateValidCustomer();
            var creditCard = CreditCard.Create("Mario", "Rossi", "08/30", "0654", customer.Id).Value;
            customer.AddCreditCard(creditCard);
            customer._domainEvents.Clear();

            var result = customer.AddCreditCard(creditCard);

            result.IsSuccess.Should().BeTrue();
            customer._domainEvents.Should().BeEmpty();
        }

        [Fact]
        public void RemoveCreditCard_WhenCreditCardIsNull_ShouldReturnNotFoundFailure()
        {
            var customer = CreateValidCustomer();

            var result = customer.RemoveCreditCard();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Credit card");
            result.Error.Description.Should().Be("Cannot remove null credit card");
            result.Error.Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void RemoveCreditCard_WhenCreditCardExists_ShouldSetToNullAndRaiseCreditCardRemovedEvent()
        {
            var customer = CreateValidCustomer();
            var creditCard = CreditCard.Create("Mario","Rossi","08/30","0654",customer.Id).Value;
            customer.AddCreditCard(creditCard);
            customer._domainEvents.Clear();

            var result = customer.RemoveCreditCard();

            result.IsSuccess.Should().BeTrue();
            customer.CreditCard.Should().BeNull();
            customer._domainEvents.Should().ContainSingle(e => e is CreditCardRemovedEvent);
        }

        // ==========================================
        // CHANGE PASSWORD & PROFILE TESTS
        // ==========================================
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ChangePassword_WhenNullOrWhiteSpace_ShouldReturnFailure(string? invalidPassword)
        {
            var customer = CreateValidCustomer();

            var result = customer.ChangePassword(invalidPassword!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("New password");
            result.Error.Description.Should().Be("New password hash is null or white spaces");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void ChangePassword_WithValidHash_ShouldUpdateAndRaiseCustomerPasswordChangedEvent()
        {
            var customer = CreateValidCustomer();
            customer._domainEvents.Clear();
            var newHash = "new_hashed_password_456";

            var result = customer.ChangePassword(newHash);

            result.IsSuccess.Should().BeTrue();
            customer.PasswordHash.Should().Be(newHash);
            customer._domainEvents.Should().ContainSingle(e => e is CustomerPasswordChangedEvent);
        }

        [Fact]
        public void ProfileUpdated_ShouldRaiseProfileUpdatedEvent()
        {
            var customer = CreateValidCustomer();
            customer._domainEvents.Clear();
            var updatedFields = new List<string> { "FullName", "Email" };

            customer.ProfileUpdated(updatedFields);

            customer._domainEvents.Should().ContainSingle(e => e is ProfileUpdatedEvent);
        }
    }
}
