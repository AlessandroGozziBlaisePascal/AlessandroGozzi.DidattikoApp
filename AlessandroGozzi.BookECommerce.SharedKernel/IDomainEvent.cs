using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.SharedKernel
{
    public interface IDomainEvent: INotification
    {
        DateTime OccurredOnUtc { get; }
    }

    public abstract record DomainEvent : IDomainEvent
    {
        public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    }
}
