using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers.ValueObjects
{
    public record Surname
    {
        public string Value { get; init; }

        private Surname(string value) => Value = value;

        public static Result<Surname> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<Surname>(new Error("Surname creation", "Surname cannot be null or white spaces", ErrorType.Validation));

            var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 4)
                return Result.Failure<Surname>(new Error("Surname creation", "You can have max 4 surnames", ErrorType.Validation));

            var cleanedSurname = string.Join(" ", parts);

            if (!Regex.IsMatch(cleanedSurname, @"^[a-zA-Z\u00C0-\u024F'\s-]+$"))
                return Result.Failure<Surname>(new Error("Surname creation", "Surname must contain only letters", ErrorType.Validation));

            return Result.Success(new Surname(cleanedSurname));
        }

        public override string ToString() => Value;
    }
}
