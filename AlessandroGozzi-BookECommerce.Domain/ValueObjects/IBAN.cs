using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi.BookECommerce.Domain.ValueObjects
{
    public record IBAN
    {
        public string Value { get; init; }

        private IBAN(string value)
        {
            Value = value;
        }

        public static Result<IBAN> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<IBAN>(new Error("IBAN", "IBAN cannot be null", ErrorType.Validation));

            var clearIban = value.Replace(" ", "").ToUpper();
            if(!clearIban.All(char.IsLetterOrDigit) )
                return Result.Failure<IBAN>(new Error("IBAN", "IBAN cannot have special chars", ErrorType.Validation));
            if (clearIban.Length < 15 || clearIban.Length > 34)
                return Result.Failure<IBAN>(new Error("IBAN","Invalid Iban lenght",ErrorType.Validation));

            return Result.Success(new IBAN(value));
        }
    }
}
