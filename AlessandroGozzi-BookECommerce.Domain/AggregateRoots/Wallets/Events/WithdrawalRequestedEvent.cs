using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.ValueObjects;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Wallets.Events
{
    public record WithdrawalRequestedEvent(Guid CustomerId, Money Amount, IBAN Iban): DomainEvent;
}
