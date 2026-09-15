using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi.BookECommerce.Domain.ValueObjects
{
    public record Money
    {
        public decimal Amount { get; init; }

        private Money(decimal amount) => Amount = amount;

        public static Result<Money> Create(decimal amount)
        {
            if (amount < 0)
                return Result.Failure<Money>(new Error("Money amount", "Amount cannot be negative", ErrorType.Validation));

            return Result.Success(new Money(amount));
        }

        public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount);
        public static Money operator -(Money a, Money b) => new(a.Amount - b.Amount);

        public static Money operator *(Money a, int multiplier) => new(a.Amount * multiplier);
    }
}
