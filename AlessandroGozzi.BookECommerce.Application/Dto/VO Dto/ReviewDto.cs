using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto
{
    public record ReviewDto(
        Guid CustomerId,
        int Rating,
        string ReviewerName,
        string ReviewerSurname,
        DateTime CreatedAt
        )
    { }
}
