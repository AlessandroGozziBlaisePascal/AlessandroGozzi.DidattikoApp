using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.Value_Object
{
    public record CardOwner
    {
        public Name Name { get; init; }
        public Surname Surname { get; init; }

        private CardOwner(Name name, Surname surname)
        {
            Name = name;
            Surname = surname;
        }

        public static Result<CardOwner> Create(string name, string surname)
        {
            var nameResult = Name.Create(name);
            if (nameResult.IsFailure)
            {
                return Result.Failure<CardOwner>(nameResult.Error);
            }

            var surnameResult = Surname.Create(surname);
            if(surnameResult.IsFailure)
            {
                return Result.Failure<CardOwner>(surnameResult.Error);
            }

            return Result.Success(new CardOwner(nameResult.Value, surnameResult.Value));
        }
        public override string ToString() => $"{Name.Value} {Surname.Value}";
    }
}
