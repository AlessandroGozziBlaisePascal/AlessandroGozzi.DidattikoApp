using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers.ValueObjects
{
    public record Email
    {
        public string Value { get; init; }

        private Email() { }
        private Email(string mail) => Value = mail;

        public static Result<Email> Create(string fullEmail)
        {
            if (string.IsNullOrWhiteSpace(fullEmail))
                return Result.Failure<Email>(new Error("Email creation", "Email cannot be null or white spaces", ErrorType.Validation));

            var trimmedEmail = fullEmail.Trim();

            if (!System.Net.Mail.MailAddress.TryCreate(trimmedEmail, out _))
                return Result.Failure<Email>(new Error("Email", "Email incorrect format", ErrorType.Validation));

            return Result.Success(new Email(trimmedEmail));
        }
    }
}