using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.BookInformation
{
    public record CreateBookDto(
        string ISBN,
        string Title,
        string Subject,
        decimal Price,
        string Condition,
        int SchoolYear,
        string MainPhoto
    );
}
