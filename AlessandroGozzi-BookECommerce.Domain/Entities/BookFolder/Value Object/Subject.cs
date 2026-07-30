using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Value_Object
{
    public record Subject
    {
        public string Value { get; init; }

        private Subject(string val) => Value = val;

        public static Result<Subject> Create(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return Result.Failure<Subject>(new Error("Subject empty", "Subject cannot be null", ErrorType.Validation));

            if (!val.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                return Result.Failure<Subject>(new Error("Subject", "Subject can only contains letters, digits or spaces", ErrorType.Validation));

            return Result.Success(new Subject(val));
        }
    }
}
