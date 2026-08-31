using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books.ValueObjects
{
    public record Subject
    {
        public string Value { get; init; }

        private Subject() { }

        private Subject(string val) => Value = val;

        public static Result<Subject> Create(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return Result.Failure<Subject>(new Error("Subject empty", "Subject cannot be null", ErrorType.Validation)); var trimmedval = val.Trim();
            if (!Regex.IsMatch(trimmedval, @"^[\p{L}\p{N}\s]+$"))
                return Result.Failure<Subject>(new Error("Subject", "Subject can only contains letters, digits or spaces", ErrorType.Validation));

            return Result.Success(new Subject(val));
        }
    }
}
