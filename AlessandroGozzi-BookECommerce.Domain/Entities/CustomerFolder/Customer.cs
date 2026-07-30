using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Event;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder
{
    public class Customer: Entity
    {
        public Name Name { get; private set; }
        public Surname Surname { get; private set; }
        public Email Email { get; private set; }
        public Address Address { get; private set; }
        public CreditCard? CreditCard { get; private set; }
        public PhoneNumber Number { get; private set; }
        public TaxCode TaxCode { get; init; }
        public string PasswordHash { get; private set; }

        private Customer() { }

        private Customer(Name name, Surname surname, Email email, Address address, PhoneNumber number, TaxCode tCode, string passwordHash)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Address = address;
            Number = number;
            TaxCode = tCode;
            PasswordHash = passwordHash;
        }

        public static Result<Customer> Create(
            Name name,
            Surname surname,
            Email email,
            Address address,
            PhoneNumber number,
            TaxCode tCode,
            string passwordHash)
        {
            if (name is null)
                return Result.Failure<Customer>(new Error("Customer.Create", "Name cannot be null", ErrorType.Validation));
            if (surname is null)
                return Result.Failure<Customer>(new Error("Customer.Create", "Surname cannot be null", ErrorType.Validation));
            if (email is null)
                return Result.Failure<Customer>(new Error("Customer.Create", "Email cannot be null", ErrorType.Validation));
            if (address is null)
                return Result.Failure<Customer>(new Error("Customer.Create", "Address cannot be null", ErrorType.Validation));
            if (number is null)
                return Result.Failure<Customer>(new Error("Customer.Create", "Phone number cannot be null", ErrorType.Validation));
            if (tCode is null)
                return Result.Failure<Customer>(new Error("Customer.Create", "TaxCode cannot be null", ErrorType.Validation));
            if (string.IsNullOrWhiteSpace(passwordHash))
                return Result.Failure<Customer>(new Error("Customer.Create", "Password hash cannot be null or empty", ErrorType.Validation));

            var customer = new Customer(name, surname, email, address, number, tCode, passwordHash);

            customer.Raise(new CustomerCreatedEvent(customer.Id));

            return Result.Success(customer);
        }

        public Result ChangeName(Name name)
        {
            if (name is null)
                return Result.Failure(new Error("Change name", "Cannot change to a null value", ErrorType.Validation));

            if (Name == name)
                return Result.Success();

            var oldName = Name;
            Name = name;

            Raise(new NameChangedEvent(Id, oldName, Name));
            return Result.Success();
        }

        public Result ChangeSurname(Surname surname)
        {
            if (surname is null)
                return Result.Failure(new Error("Change surname", "Cannot change to a null value", ErrorType.Validation));

            if (Surname == surname)
                return Result.Success();

            var oldSurname = Surname;
            Surname = surname;

            Raise(new SurnameChangedEvent(Id, oldSurname, Surname));
            return Result.Success();
        }

        public Result ChangeEmail(Email mail)
        {
            if (mail is null)
                return Result.Failure(new Error("Change email", "Cannot change to a null value", ErrorType.Validation));

            if (Email == mail)
                return Result.Success();

            var oldEmail = Email;
            Email = mail;

            Raise(new EmailChangedEvent(Id, oldEmail, Email));
            return Result.Success();
        }

        public Result ChangeNumber(PhoneNumber number)
        {
            if (number is null)
                return Result.Failure(new Error("Change number", "Cannot change to a null value", ErrorType.Validation));

            if (Number == number)
                return Result.Success();

            var oldNumber = Number;
            Number = number;

            Raise(new NumberChangedEvent(Id, oldNumber, Number));
            return Result.Success();
        }

        public Result ChangeAddress(Address address)
        {
            if (address is null)
                return Result.Failure(new Error("Change address", "Cannot change to a null value", ErrorType.Validation));

            if (Address == address)
                return Result.Success();

            var oldAddress = Address;
            Address = address;

            Raise(new AddressChangedEvent(Id, oldAddress, Address));
            return Result.Success();
        }

        public Result AddCreditCard(CreditCard card)
        {
            if (card is null)
                return Result.Failure(new Error("Credit card", "Cannot add null credit card", ErrorType.Validation));

            if (CreditCard == card)
                return Result.Success();

            CreditCard = card;
            Raise(new CreditCardAddedEvent(Id, card.CardOwner, card.DisplayName));

            return Result.Success();
        }

        public Result ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                return Result.Failure(new Error("New password", "New password hash is null or white spaces", ErrorType.Validation));

            if (PasswordHash == newPasswordHash)
                return Result.Success();

            var oldPasswordHash = PasswordHash;
            PasswordHash = newPasswordHash;

            Raise(new CustomerPasswordChangedEvent(Id, oldPasswordHash, PasswordHash));
            return Result.Success();
        }
    }
}
