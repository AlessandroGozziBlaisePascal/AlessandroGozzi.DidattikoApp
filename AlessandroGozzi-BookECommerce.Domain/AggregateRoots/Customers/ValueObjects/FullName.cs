using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers.ValueObjects
{
    public record FullName
    {
        public Name Name { get; init; }
        public Surname Surname { get; init; }

        public FullName(Name name, Surname surname)
        {
            Name = name;
            Surname = surname;
        }

        public override string ToString() => $"{Name} {Surname}";
    }
}
