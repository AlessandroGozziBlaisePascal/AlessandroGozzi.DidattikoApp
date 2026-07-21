using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder
{
    public sealed class CreditCard: Entity
    {
        public CardOwner CardOwner { get; init; }
        public ExpiryDate ExpiryDate { get; init; }
        public string Last4Digits { get; init; }
        public string PaymentToken { get; init; }

        public string DisplayName => $"XXXX-XXXX-XXXX-{Last4Digits}";

        private CreditCard() { }

        private CreditCard(CardOwner owner, ExpiryDate date, string last4digits, string token)
        {
            CardOwner = owner;
            ExpiryDate = date;
            Last4Digits = last4digits;
            PaymentToken = token;
        }
        public static Result<CreditCard> Create(string rawName, string rawSurname, string rawExpiryDate, string last4digits, string token)
        {
            var ownerResult = CardOwner.Create(rawName, rawSurname);
            if (ownerResult.IsFailure)
            {
                return Result.Failure<CreditCard>(ownerResult.Error);
            }

            var expiryDateResult = ExpiryDate.Create(rawExpiryDate);
            if(expiryDateResult.IsFailure)
            {
                return Result.Failure<CreditCard>(expiryDateResult.Error);
            }

            if(string.IsNullOrWhiteSpace(last4digits) || last4digits.Length != 4)
                return Result.Failure<CreditCard>(new Error("Last 4 digits card", "Card must have 4 last digits", ErrorType.Validation));
            if(string.IsNullOrWhiteSpace(token))
                return Result.Failure<CreditCard>(new Error("Card token", "Must have card token", ErrorType.Validation));

            return Result.Success(new CreditCard(ownerResult.Value, expiryDateResult.Value, last4digits, token));
        }
    }
}
