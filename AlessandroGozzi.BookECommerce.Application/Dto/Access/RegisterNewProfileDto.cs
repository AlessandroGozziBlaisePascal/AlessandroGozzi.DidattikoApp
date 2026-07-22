using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Access
{
    public record RegisterNewProfileDto(
        string Name,
        string Surname,
        string Email,
        string PhoneNumber,
        string Password,
        string ConfirmPassword
    )
    {
        public override string ToString() =>
            $"RegisterNewProfileDto {{ Name = {Name}, Surname = {Surname}, Email = {Email}, PhoneNumber = {PhoneNumber}, Password = [PROTECTED]}}";
    }
}
