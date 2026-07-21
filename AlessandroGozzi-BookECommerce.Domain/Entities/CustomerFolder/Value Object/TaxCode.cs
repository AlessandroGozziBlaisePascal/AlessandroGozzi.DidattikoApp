using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object
{
    public record TaxCode
    {
        public string Value { get; init; }

        private TaxCode(string val) => Value = val;

        public static Result<TaxCode> Create(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return Result.Failure<TaxCode>(new Error("Tax code creation", "Tax code cannot be null or white spaces", ErrorType.Validation));
            var cleanedCode = val.Trim().ToUpper();
            if (cleanedCode.Length != 16)
                return Result.Failure<TaxCode>(new Error("Tax code creation", "Tax code lenght must be 16 digits", ErrorType.Validation));
            if (!cleanedCode.All(char.IsLetterOrDigit))
                return Result.Failure<TaxCode>(new Error("Tax code creation", "Tax code must be comped by letters or digits", ErrorType.Validation));

            return Result.Success(new TaxCode(cleanedCode));
        }
    }
}
