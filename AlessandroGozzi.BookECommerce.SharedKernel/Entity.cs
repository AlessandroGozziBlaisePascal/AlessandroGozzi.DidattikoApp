namespace AlessandroGozzi.BookECommerce.SharedKernel
{
    public class Entity
    {
        public Guid Id { get; set; }

        private readonly List<IDomainEvent> _domainEvents;

        public Entity()
        {
            Id = Guid.NewGuid();
            _domainEvents = new List<IDomainEvent>();
        }

        public Entity(Guid id)
        {
            Id = id;
            _domainEvents = new List<IDomainEvent>();
        }

        protected void Raise(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearEvents() => _domainEvents.Clear();
    }
}
