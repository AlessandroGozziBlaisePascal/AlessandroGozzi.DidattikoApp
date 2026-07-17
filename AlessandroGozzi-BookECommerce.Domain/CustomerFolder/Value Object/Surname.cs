using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object
{
    public record Surname
    {
        public string Value { get; init; }
        public Surname(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Surname cannot be null or empty.", nameof(value));
            var trimmedSurname = value.Trim();
            if (!Regex.IsMatch(trimmedSurname, @"^[a-zA-ZÀ-ÿ'\s\-]+$"))
                throw new ArgumentException("Surname must be composed by letters", nameof(value));
            Value = trimmedSurname;
        }

    }
}
