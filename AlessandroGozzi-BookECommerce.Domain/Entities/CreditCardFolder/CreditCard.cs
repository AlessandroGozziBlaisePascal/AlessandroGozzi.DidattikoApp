using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder
{
    public sealed class CreditCard: Entity
    {
        public Guid CustomerId { get; private set; }
        public FullName CardOwner { get; init; }
        public ExpiryDate ExpiryDate { get; init; }
        public string Last4Digits { get; init; }

        public string DisplayName => $"XXXX-XXXX-XXXX-{Last4Digits}";

        private CreditCard() { }

        private CreditCard(FullName owner, ExpiryDate date, string last4digits, Guid customerId)
        {
            CardOwner = owner;
            ExpiryDate = date;
            Last4Digits = last4digits;
            CustomerId = customerId;
        }
        public static Result<CreditCard> Create(string rawName, string rawSurname, string rawExpiryDate, string last4digit, Guid custId)
        {
            var nameResult = Name.Create(rawName);
            if (nameResult.IsFailure)
                return Result.Failure<CreditCard>(new Error("Owner name", "Incorrect owner name", ErrorType.Validation));

            var surnameResult = Surname.Create(rawSurname);
            if (surnameResult.IsFailure)
                return Result.Failure<CreditCard>(new Error("Owner surname", "Incorrect owner surname", ErrorType.Validation));

            var expiryDateResult = ExpiryDate.Create(rawExpiryDate);
            if(expiryDateResult.IsFailure)
            {
                return Result.Failure<CreditCard>(expiryDateResult.Error);
            }

            if(string.IsNullOrWhiteSpace(last4digit) || last4digit.Length != 4 || !last4digit.All(char.IsDigit))
                return Result.Failure<CreditCard>(new Error("Last 4 digits card", "Card must have 4 last digits", ErrorType.Validation));
            if (custId == Guid.Empty)
                return Result.Failure<CreditCard>(new Error("Customer id", "Customer id cannot be null", ErrorType.Validation));

            return Result.Success(new CreditCard(new FullName(nameResult.Value, surnameResult.Value), expiryDateResult.Value, last4digit, custId));
        }

        public bool IsExpired(DateTime? referenceDate = null) => ExpiryDate.IsExpired(referenceDate);
    }
}
