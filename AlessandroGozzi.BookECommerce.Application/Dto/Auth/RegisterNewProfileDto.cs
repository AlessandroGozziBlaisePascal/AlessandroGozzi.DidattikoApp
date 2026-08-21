using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Auth
{
    public record RegisterNewProfileDto(
        string Name,
        string Surname,
        string Email,
        string PhoneNumber,
        AddressDto Address,
        string TaxCode,
        string Password,
        string ConfirmPassword
    )
    {
        public override string ToString() =>
            $"RegisterNewProfileDto {{ Name = {Name}, Surname = {Surname}, Email = {Email}, PhoneNumber = {PhoneNumber}, Address = {Address}, TaxCode = {TaxCode}, Password = [PROTECTED]}}";
    }
}
