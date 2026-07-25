using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;

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
