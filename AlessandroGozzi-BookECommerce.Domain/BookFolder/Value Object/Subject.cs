using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.BookFolder.Value_Object
{
    public record Subject
    {
        public string Value { get; init; }

        private Subject(string val)
        {
            Value = val;
        }

        public static Result<Subject> Create(string val)
        {
            
            if (val == null)
            {
                return Result.Failure<Subject>(new Error("Subject", "Subject cannot be null", ErrorType.Validation));
            }
            return val.All(char.IsLetterOrDigit)
                ? Result.Success(new Subject(val))
                : Result.Failure<Subject>(new Error("Subject", "Subject must have only digits or letters", ErrorType.Validation));
        }
    }
}
