using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto
{
    public record BookDto(
        Guid BookId,
        string ISBN,
        string Title,
        string Condition,
        string MainPhoto,
        decimal Price,
        bool IsAvailable,
        Guid SellerId
    );
}
