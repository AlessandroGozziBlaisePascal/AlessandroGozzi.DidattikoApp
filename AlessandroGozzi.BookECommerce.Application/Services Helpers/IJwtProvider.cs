using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;

namespace AlessandroGozzi.BookECommerce.Application.Services_Helpers
{
    public interface IJwtProvider
    {
        string GenerateToken(CustomerDto customer);
    }
}
