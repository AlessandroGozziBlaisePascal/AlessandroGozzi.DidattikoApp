using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object
{
    public record Surname
    {
        public string Value { get; init; }

        private Surname(string value) => Value = value;

        public static Result<Surname> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<Surname>(new Error("Surname creation", "Surname cannot be null ro white spaces", ErrorType.Validation));
            var surnameCounter = value.Split(' ',StringSplitOptions.RemoveEmptyEntries);
            if(surnameCounter.Length > 4)
                return Result.Failure<Surname>(new Error("Surname creation", "You can have max 4 surnames", ErrorType.Validation));
            var trimmedSurname = value.Trim();

            if (!Regex.IsMatch(trimmedSurname, @"^[a-zA-Z\u00C0-\u024F'\s-]+$"))
                return Result.Failure<Surname>(new Error("Surname creation", "Surname must contain only letters", ErrorType.Validation));

            return Result.Success(new Surname(trimmedSurname));
        }
    }
}
