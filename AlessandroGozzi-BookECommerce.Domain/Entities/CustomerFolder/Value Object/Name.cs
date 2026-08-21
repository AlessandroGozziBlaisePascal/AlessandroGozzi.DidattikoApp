using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object
{
    public record Name
    {
        public string Value { get; init; }

        private Name(string value) => Value = value;

        public static Result<Name> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<Name>(new Error("Name creation", "Name cannot be null or white spaces", ErrorType.Validation));
            var nameCounter = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (nameCounter.Length > 3)
                return Result.Failure<Name>(new Error("Name creation", "You can have max 3 names", ErrorType.Validation));
            var trimmedName = value.Trim();

            if (!Regex.IsMatch(trimmedName, @"^[a-zA-Z\u00C0-\u024F'\s-]+$"))
                return Result.Failure<Name>(new Error("Name creation", "Name must contain only letters", ErrorType.Validation));

            return Result.Success(new Name(trimmedName));
        }

    }
}
