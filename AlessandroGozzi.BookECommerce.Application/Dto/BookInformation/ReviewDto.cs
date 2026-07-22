using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.BookInformation
{
    public record ReviewDto(
        string Comment,
        int Rating,
        string ReviewerName,
        string ReviewerSurname,
        DateTime CreatedAt
        )
    { }
}
