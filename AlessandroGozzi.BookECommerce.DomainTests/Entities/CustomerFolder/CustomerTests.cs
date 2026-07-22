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
            var result = Customer.Create(
                GetValidName(),
                GetValidSurname(),
                GetValidEmail(),
                GetValidAddress(),
                GetValidPhoneNumber(),
                GetValidTaxCode()
            );

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Value.Should().Be("Mario");
            result.Value.Surname.Value.Should().Be("Rossi");
            result.Value.Email.Value.Should().Be("mario.rossi@example.com");

            result.Value._domainEvents.Should().HaveCount(1);
        }

        #endregion

        #region Change Methods Tests

        [Fact]
        public void ChangeName_WithValidName_ShouldUpdateAndRaiseEvent()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode()
            ).Value;

            var newName = Name.Create("Luigi").Value;

            var result = customer.ChangeName(newName);

            result.IsSuccess.Should().BeTrue();
            customer.Name.Value.Should().Be("Luigi");
            customer._domainEvents.Should().HaveCount(2); 
        }

        [Fact]
        public void ChangeName_WithNullValue_ShouldFail()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode()
            ).Value;

            var result = customer.ChangeName(null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Change name");
        }

        [Fact]
        public void ChangeName_WithSameValue_ShouldDoNothingAndSucceed()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode()
            ).Value;

            var sameName = GetValidName(); 

            var result = customer.ChangeName(sameName);

            result.IsSuccess.Should().BeTrue();
            customer._domainEvents.Should().HaveCount(1); 
        }

        [Fact]
        public void ChangeEmail_WithValidEmail_ShouldUpdateAndRaiseEvent()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode()
            ).Value;

            var newEmail = Email.Create("nuova.email@example.com").Value;

            var result = customer.ChangeEmail(newEmail);

            result.IsSuccess.Should().BeTrue();
            customer.Email.Value.Should().Be("nuova.email@example.com");
        }
        [Fact]
        public void AddCreditCard_ShouldReturnFailure_WhenCardIsNull()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode()
            ).Value;
            CreditCard nullCard = null;

            var result = customer.AddCreditCard(nullCard);

            result.IsSuccess.Should().BeFalse(); 
            result.Error.Description.Should().Be("Cannot add null credit card");
        }

        [Fact]
        public void AddCreditCard_ShouldReturnSuccessWithoutRaisingEvent_WhenCardIsAlreadyAssigned()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode()
            ).Value;
            var card = CreditCard.Create("Alessandro", "Gozzi", "10/30", "0672", "token_10292").Value;

            customer.AddCreditCard(card);
            customer.ClearEvents(); 

            var result = customer.AddCreditCard(card);

            result.IsSuccess.Should().BeTrue();
            customer._domainEvents.Should().NotContain(e => e is CreditCardAddedEvent); 
        }

        [Fact]
        public void AddCreditCard_ShouldSetCreditCardAndRaiseEvent_WhenCardIsValidAndNew()
        {
            var customer = Customer.Create(
                GetValidName(), GetValidSurname(), GetValidEmail(),
                GetValidAddress(), GetValidPhoneNumber(), GetValidTaxCode()
            ).Value;

            var card = CreditCard.Create("Alessandro", "Gozzi", "10/30", "0672", "token_10292").Value;
            customer.AddCreditCard(card);
            customer.ClearEvents();

            var card2 = CreditCard.Create("Noemi", "Colciago", "10/31", "0670", "token_10772").Value;

            var result = customer.AddCreditCard(card2);

            result.IsSuccess.Should().BeTrue();
            customer.CreditCard.Should().Be(card2);

            customer._domainEvents.Should().ContainSingle(e => e is CreditCardAddedEvent);
        }


        #endregion

        #region Value Objects Unit Tests

        [Theory]
        [InlineData("2010")] 
        [InlineData("201000")] 
        [InlineData(null)]
        [InlineData("")]
        public void Address_Create_WithInvalidCAP_ShouldFail(string invalidCap)
        {
            var result = Address.Create("Via Roma", "10", "Milano", invalidCap);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Address CAP");
        }

        [Theory]
        [InlineData("RSSMRA80A01H50")] 
        [InlineData("RSSMRA80A01H501!")] 
        public void TaxCode_Create_WithInvalidValue_ShouldFail(string invalidTaxCode)
        {
            var result = TaxCode.Create(invalidTaxCode);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tax code creation");
        }

        [Theory]
        [InlineData("email-non-valida")]
        [InlineData("test@")]
        [InlineData("@domain.com")]
        public void Email_Create_WithInvalidFormat_ShouldFail(string invalidEmail)
        {
            var result = Email.Create(invalidEmail);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Email");
        }

        #endregion

    }
}
