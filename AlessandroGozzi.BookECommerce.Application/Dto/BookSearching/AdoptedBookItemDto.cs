using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto
{
    public record AdoptedBookItemDto(
        Guid? InternalBookId,
        string ISBN,
        string Title,
        string Authors,
        string Publisher,
        string Subject,
        decimal Price,
        bool IsMandatory,
        bool IsAvailableInCatalog
        );
}
