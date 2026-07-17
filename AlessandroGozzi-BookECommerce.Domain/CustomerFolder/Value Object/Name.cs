using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object
{
    public record Name
    {
        public string Value { get; init; }
        public Name(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be null or empty.", nameof(value));
            var trimmedName = value.Trim();
            if (!Regex.IsMatch(trimmedName, @"^[a-zA-ZÀ-ÿ'\s\-]+$"))
                throw new ArgumentException("Name must be composed by letters", nameof(value));
            Value = trimmedName;
        }

    }
}
