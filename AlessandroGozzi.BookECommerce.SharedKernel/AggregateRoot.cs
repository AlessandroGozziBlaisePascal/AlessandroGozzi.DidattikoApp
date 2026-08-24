using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.SharedKernel
{
    public abstract class AggregateRoot:Entity
    {
        public readonly List<IDomainEvent> _domainEvents;

        public AggregateRoot(): base()
        {
            _domainEvents = new List<IDomainEvent>();
        }

        public AggregateRoot(Guid id) : base(id)
        {
            _domainEvents = new List<IDomainEvent>();
        }

        protected void Raise(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearEvents() => _domainEvents.Clear();
    }
}
