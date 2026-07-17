using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object
{
    public record Email
    {
        public string FirstPart { get; init; }
        public string Provider { get; init; }
        public string Domain { get; init; }

        public Email(string firstPart, string provider, string domain)
        {
            FirstPart = firstPart;
            Provider = provider;
            Domain = domain;
        }

        public Email(string fullEmail)
        {
            if (string.IsNullOrWhiteSpace(fullEmail))
                throw new ArgumentNullException(nameof(fullEmail));
            var firstSplit = fullEmail.Split('@');
            if (firstSplit.Length != 2)
                throw new ArgumentException("Invalid Email: must contain exact one @");
            FirstPart = firstSplit[0];
            var secondSplit = firstSplit[1].Split('.');
            if (secondSplit.Length != 2)
                throw new ArgumentException("Invalid Email: Provider part must contain only one .");
            Provider = secondSplit[0];
            Domain = secondSplit[1];
        }
    }
}
