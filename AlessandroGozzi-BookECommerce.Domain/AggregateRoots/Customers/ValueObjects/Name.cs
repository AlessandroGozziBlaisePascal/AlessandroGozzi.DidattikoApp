using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers.ValueObjects
{
    public record Name
    {
        public string Value { get; init; }

        private Name(string value) => Value = value;

        public static Result<Name> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<Name>(new Error("Name creation", "Name cannot be null or white spaces", ErrorType.Validation));

            var nameParts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length > 3)
                return Result.Failure<Name>(new Error("Name creation", "You can have max 3 names", ErrorType.Validation));

            var cleanedName = string.Join(" ", nameParts);

            if (!Regex.IsMatch(cleanedName, @"^[a-zA-Z\u00C0-\u024F'\s-]+$"))
                return Result.Failure<Name>(new Error("Name creation", "Name must contain only letters", ErrorType.Validation));

            return Result.Success(new Name(cleanedName));
        }

        public override string ToString() => Value;

    }
}
