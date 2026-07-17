using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Event;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder
{
    public class Customer: Entity
    {
        public Name Name { get; private set; }
        public Surname Surname { get; private set; }
        public Email Email { get; private set; }
        public Address Address {  get; private set; }
        private Cart Cart { get; set; }
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
            Cart = new Cart();
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
            var n = Name;
            Name = name;
            Raise(new NameChangedEvent(Id, n, Name));
            return Result.Success();
        }

        public Result ChangeSurname(Surname surname)
        {
            if (surname == null)
                return Result.Failure(new Error("Change surname", "Cannot change to a null value", ErrorType.Validation));
            var sn = Surname;
            Surname = surname;
            Raise(new SurnameChangedEvent(Id, sn, Surname));
            return Result.Success();
        }

        public Result ChangeEmail(Email mail)
        {
            if (mail == null)
                return Result.Failure(new Error("Change email", "Cannot change to a null value", ErrorType.Validation));
            var m = Email;
            Email = mail;
            Raise(new EmailChangedEvent(Id, m, Email));
            return Result.Success();
        }

        public Result ChangeNumber(PhoneNumber number)
        {
            if (number == null)
                return Result.Failure(new Error("Change number", "Cannot change to a null value", ErrorType.Validation));
            var n = Number;
            Number = number;
            Raise(new NumberChangedEvent(Id, n, Number));
            return Result.Success();
        }

        public Result ChangeAddress(Address address)
        {
            if (address == null)
                return Result.Failure(new Error("Change address", "Cannot change to a null value", ErrorType.Validation));
            var a = Address;
            Address = address;
            Raise(new AddressChangedEvent(Id, a, Address));
            return Result.Success();
        }

        public Result AddBookToCart(Book book, int quantity)
        {
            Cart.AddBook(book, quantity);
            Raise(new BookAddedToCartEvent(Id, book.Id, book.Name, book.Price));
            return Result.Success();
        }
        public Result RemoveBookFromCart(Book book, int quantity)
        {
            Cart.RemoveBook(book, quantity);
            Raise(new BookRemovedFromCartEvent(Id, book.Id, book.Name, book.Price);
            return Result.Success();
        }
    }
}
