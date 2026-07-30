using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.Value_Object
{
    public record ExpiryDate
    {
        public int Month { get; init; }
        public int Year { get; init; }

        private ExpiryDate(int month, int year)
        {
            Month = month;
            Year = year;
        }

        public static Result<ExpiryDate> Create(string rawDate)
        {
            if(string.IsNullOrWhiteSpace(rawDate) || !Regex.IsMatch(rawDate, @"^(0[1-9]|1[0-2])/\d{2}$"))
            {
                return Result.Failure<ExpiryDate>(new Error("Expiry date", "No valid format", ErrorType.Validation));
            }
            var parts = rawDate.Split('/');
            return Result.Success(new ExpiryDate(int.Parse(parts[0]), 2000 + int.Parse(parts[1])));
        }

        public bool IsExpired(DateTime referenceDate)
        {
            var lastDayOfMonth = new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month), 23, 59, 59);
            return lastDayOfMonth < referenceDate;
        }

        public override string ToString() => $"{Month}/{Year}";
    }
}
