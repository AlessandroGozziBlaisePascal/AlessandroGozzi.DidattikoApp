using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object
{
    public record PhoneNumber
    {
        public string Value { get; init; }

        private PhoneNumber(string n) => Value = n;

        public string InternationNumber => $"+39{Value}";

        public static Result<PhoneNumber> Create(string n)
        {
            if (string.IsNullOrWhiteSpace(n))
                return Result.Failure<PhoneNumber>(new Error("Phone number creation", "Number cannot be null or white spaces", ErrorType.Validation));
            var cleanedN = n.Trim().Replace(" ", "").Replace("-", "").Replace("+39", "").Replace("0039", "");
            if (cleanedN.Length < 9 || cleanedN.Length > 11)
                return Result.Failure<PhoneNumber>(new Error("Phone number creation", "Phone number must be between 9 and 11 digits", ErrorType.Validation));
            if (!long.TryParse(cleanedN, out _))
                return Result.Failure<PhoneNumber>(new Error("Phone number creation", "Phone must be composed by only digits", ErrorType.Validation));
            
            return Result.Success(new PhoneNumber(cleanedN));
        }
    }
}
