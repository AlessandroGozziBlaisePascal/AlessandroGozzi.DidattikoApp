using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.CustomerEvent;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder
{
    public class Customer: Entity
    {
        public FullName FullName { get; private set; }
        public Email Email { get; private set; }
        public Address Address {  get; private set; }
        public CreditCard? CreditCard { get; private set; }
        public Wallet Wallet { get; private set; }
        public PhoneNumber Number { get; private set; }
        public TaxCode TaxCode { get; init; }
        public string PasswordHash { get; private set;}

        private Customer(FullName fullName, Email email, Address address, PhoneNumber number, TaxCode TCode, string passwordHash)
        {
            FullName = fullName;
            Email = email;
            Address = address;
            Number = number;
            TaxCode = TCode;
            PasswordHash = passwordHash;
            Wallet = new Wallet(Id);
        }

        public static Result<Customer> Create(FullName fullName, Email email, Address address, PhoneNumber number, TaxCode TCode, string passwordHash)
        {
            var customer = new Customer(fullName, email, address, number, TCode, passwordHash);

            customer.Raise(new CustomerCreatedEvent(customer.Id));

            return Result.Success(customer);
        }

        public Result ChangeName(FullName newFullname)
        {
            if (newFullname == null)
                return Result.Failure(new Error("Change name", "Cannot change to a null value", ErrorType.Validation));
            if (FullName == newFullname)
                return Result.Success();
            var n = FullName;
            FullName = newFullname;
            Raise(new NameChangedEvent(Id, n, FullName));
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

        public Result AddCreditCard(CreditCard card)
        {
            if(card == null)
                return Result.Failure(new Error("Credit card","Cannot add null credit card",ErrorType.Validation)); 
            if(card == CreditCard)
                return Result.Success();

            CreditCard = card;
            Raise(new CreditCardAddedEvent(Id, card.CardOwner, card.DisplayName));
            return Result.Success();
        }
        public Result RemoveCreditCard()
        {
            if (CreditCard == null)
                return Result.Failure(new Error("Credit card", "Cannot remove null credit card", ErrorType.NotFound));
            var oldCard = CreditCard;
            CreditCard = null;
            Raise(new CreditCardRemovedEvent(Id, oldCard.CardOwner, oldCard.DisplayName));
            return Result.Success();
        }

        public Result ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
            {
                return Result.Failure(new Error("New password", "New password hash is null or white spaces",ErrorType.Validation));
            }

            PasswordHash = newPasswordHash;
            Raise(new CustomerPasswordChangedEvent(Id));
            return Result.Success();
        }

        public void ProfileUpdated(List<string> updatedFields)
        {
            Raise(new ProfileUpdatedEvent(Id, updatedFields));
        }
    }
}
