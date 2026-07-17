using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object
{
    public class Money
    {
        public decimal Amount { get; private set; }

        private Money(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.", nameof(amount));
            Amount = amount;
        }

        public static Money Create(decimal amount) => new(amount);

        public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount);

        public static Money operator *(Money a, int multiplier) => new(a.Amount * multiplier);
    }
}
