using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object
{
    public record PhoneNumber
    {
        public string Value { get; init; }

        public PhoneNumber(string n)
        {
            if (string.IsNullOrWhiteSpace(n))
                throw new ArgumentNullException(nameof(n));
            var cleanedN = n.Trim().Replace(" ", "").Replace("-","").Replace("+39", "").Replace("0039", "");
            if (cleanedN.Length < 9 || cleanedN.Length > 11)
                throw new ArgumentException("Italian number needs from 9 to 11 digits");
            if (!long.TryParse(cleanedN, out _))
                throw new ArgumentException("Number must contains only digits");
            Value = cleanedN;
        }

        public string InternationNumber => $"+39{Value}";
    }
}
