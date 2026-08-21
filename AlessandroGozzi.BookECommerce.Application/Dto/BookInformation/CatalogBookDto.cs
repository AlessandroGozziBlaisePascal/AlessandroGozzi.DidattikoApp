using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.BookInformation
{
    public record CatalogBookDto(
        Guid BookId,
        string ISBN,
        string Condition,
        string MainPhoto,
        decimal Price,
        bool IsAvailable
        );
}
