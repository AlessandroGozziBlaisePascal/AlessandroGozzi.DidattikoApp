using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.BookFolder.Value_Object
{
    public record ISBN
    {
        public string Value { get; init; }

        private ISBN(string value)
        {
            Value = value;
        }

        public static Result<ISBN> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<ISBN>(new Error("ISBN.Empty", "ISBN cannot be empty.", ErrorType.Validation));
            }

            var cleaned = value.Replace("-", "").Replace(" ", "").Trim().ToUpper();

            if (!Regex.IsMatch(cleaned, @"^(\d{13}|\d{9}[\dX])$"))
            {
                return Result.Failure<ISBN>(new Error("ISBN.InvalidFormat", "ISBN contains invalid characters or has an incorrect length.", ErrorType.Validation));
            }

            if (cleaned.Length == 13 && IsValidIsbn13(cleaned))
            {
                return Result.Success(new ISBN(cleaned));
            }

            if (cleaned.Length == 10 && IsValidIsbn10(cleaned))
            {
                return Result.Success(new ISBN(cleaned));
            }

            return Result.Failure<ISBN>(new Error("ISBN.InvalidCheckDigit", "The provided ISBN code is mathematically invalid.", ErrorType.Validation));
        }


        private static bool IsValidIsbn13(string isbn)
        {
            if (!Regex.IsMatch(isbn, @"^\d{13}$")) return false;

            // Verifica algoritmo Check Digit (Pesi alternati 1 e 3)
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = isbn[i] - '0';
                sum += (i % 2 == 0) ? digit : digit * 3;
            }

            int remainder = sum % 10;
            int checkDigit = (10 - remainder) % 10;

            return checkDigit == (isbn[12] - '0');
        }

        private static bool IsValidIsbn10(string isbn)
        {
            if (!Regex.IsMatch(isbn, @"^\d{9}[\dX]$")) return false;

            // Verifica algoritmo Check Digit (Pesi decrescenti da 10 a 2)
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (isbn[i] - '0') * (10 - i);
            }

            char lastChar = isbn[9];
            int lastValue = (lastChar == 'X') ? 10 : (lastChar - '0');

            return (sum + lastValue) % 11 == 0;
        }

    }
}
