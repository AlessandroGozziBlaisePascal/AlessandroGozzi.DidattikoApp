using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.BookInformation
{
    public record SpecificBookInformationDto(
        string ISBN,
        string Title,
        string Subject,
        int SchoolYear,
        int PublishedYear,
        string Condition,
        string MainPhoto,
        IReadOnlyCollection<string> Photos,
        IReadOnlyCollection<ReviewDto> Reviews,
        decimal Price,
        string SellerName,
        string SellerSurname
        )
    {
    }
}
