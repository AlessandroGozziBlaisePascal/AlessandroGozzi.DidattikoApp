using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Access
{
    public record LoginResponseDto(
        string AccessToken,            
        DateTime ExpiresAt,            
        string? RefreshToken,
        CustomerDto Customer,
        bool HasSavedCreditCard,
        string? MaskedCardNumbers
        ) { }
}
