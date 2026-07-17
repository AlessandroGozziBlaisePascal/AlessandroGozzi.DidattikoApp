using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object
{
    public record TaxCode
    {
        public string Value { get; init; }

        public TaxCode(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                throw new ArgumentNullException(nameof(val));
            var cleanedCode = val.Trim().ToUpper();
            if (cleanedCode.Length != 16)
                throw new ArgumentException("Tax code must be 16 chars");
            if (!cleanedCode.All(char.IsLetterOrDigit))
                throw new ArgumentException("Tax code can have only letters or numbers");
            Value = cleanedCode;
        }
    }
}
