using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object
{
    public record Address
    {
        public string Street { get; init; }
        public string CivicNumber { get; init; }
        public string City { get; init; }
        public string CAP { get; init; }

        private Address(string street, string civicN, string city, string cap)
        {
            Street = street;
            CivicNumber = civicN;
            City = city;
            CAP = cap;
        }
        public static Result<Address> Create(string street, string civicN, string city, string cap)
        {
            if (string.IsNullOrWhiteSpace(street))
                return Result.Failure<Address>(new Error("Address street", "Street cannot be null", ErrorType.Validation));
            if (string.IsNullOrWhiteSpace(civicN))
                return Result.Failure<Address>(new Error("Address civic number", "Civic number cannot be null", ErrorType.Validation));
            if (string.IsNullOrWhiteSpace(city))
                return Result.Failure<Address>(new Error("Address city", "City cannot be null", ErrorType.Validation));
            if (string.IsNullOrWhiteSpace(cap) || cap.Length != 5)
                return Result.Failure<Address>(new Error("Address CAP", "Street cannot be null", ErrorType.Validation));
            return Result.Success(new Address(street, civicN, city, cap));
        }
        public override string ToString() => $"{Street} {CivicNumber}, {City} {CAP}";
    }
}
