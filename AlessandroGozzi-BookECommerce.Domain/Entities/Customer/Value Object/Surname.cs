using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.Customer.Value_Object
{
    public record Surname
    {
        public string Value { get; init; }
        private Surname(string value) => Value = value;

        public static Result<Surname> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<Surname>(new Error("Surname creation", "Surname cannot be null ro white spaces", ErrorType.Validation));
            var trimmedSurname = value.Trim();
            if (!Regex.IsMatch(trimmedSurname, @"^[a-zA-ZÀ-ÿ'\s\-]+$"))
                return Result.Failure<Surname>(new Error("Surname creation", "Surname must contains only letters", ErrorType.Validation));

            return Result.Success(new Surname(trimmedSurname));
        }

    }
}
