using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Auth
{
    public record RegisterNewProfileDto(
        string FullName,
        string Email,
        string PhoneNumber,
        AddressDto Address,
        string Password,
        string ConfirmPassword
    );
}
