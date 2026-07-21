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
        public Address Address {  get; private set; }
        private CreditCard CreditCard { get; set; }
        public PhoneNumber Number { get; private set; }
        public TaxCode TaxCode { get; init; }

        private Customer(Name name, Surname surname, Email email, Address address, CreditCard card, PhoneNumber number, TaxCode TCode)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Address = address;
            CreditCard = card;
            Number = number;
            TaxCode = TCode;
        }

        public static Result<Customer> Create(Name name, Surname surname, Email email, Address address, CreditCard card, PhoneNumber number, TaxCode TCode)
        {
            var customer = new Customer(name, surname, email, address, card, number, TCode);

            customer.Raise(new CustomerCreatedEvent(customer.Id));

            return Result.Success(customer);
        }

        public Result ChangeName(Name name)
        {
            if (name == null)
                return Result.Failure(new Error("Change name", "Cannot change to a null value", ErrorType.Validation));
            if (Name == name)
                return Result.Success();
            var n = Name;
            Name = name;
            Raise(new NameChangedEvent(Id, n, Name));
            return Result.Success();
        }

        public Result ChangeSurname(Surname surname)
        {
            if (surname == null)
                return Result.Failure(new Error("Change surname", "Cannot change to a null value", ErrorType.Validation));
            if (Surname == surname)
                return Result.Success();
            var sn = Surname;
            Surname = surname;
            Raise(new SurnameChangedEvent(Id, sn, Surname));
            return Result.Success();
        }

        public Result ChangeEmail(Email mail)
        {
            if (mail == null)
                return Result.Failure(new Error("Change email", "Cannot change to a null value", ErrorType.Validation));
            if(Email == mail)
                return Result.Success();
            var m = Email;
            Email = mail;
            Raise(new EmailChangedEvent(Id, m, Email));
            return Result.Success();
        }

        public Result ChangeNumber(PhoneNumber number)
        {
            if (number == null)
                return Result.Failure(new Error("Change number", "Cannot change to a null value", ErrorType.Validation));
            if (Number == number)
                return Result.Success();
            var n = Number;
            Number = number;
            Raise(new NumberChangedEvent(Id, n, Number));
            return Result.Success();
        }

        public Result ChangeAddress(Address address)
        {
            if (address == null)
                return Result.Failure(new Error("Change address", "Cannot change to a null value", ErrorType.Validation));
            if (Address == address)
                return Result.Success();
            var a = Address;
            Address = address;
            Raise(new AddressChangedEvent(Id, a, Address));
            return Result.Success();
        }
    }
}
