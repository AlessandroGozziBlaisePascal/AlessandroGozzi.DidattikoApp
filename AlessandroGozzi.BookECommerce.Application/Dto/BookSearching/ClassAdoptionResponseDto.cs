using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.BookSearching
{
    public record ClassAdoptionResponseDto(string SchoolName, string ClassName, string AcademicYear, IReadOnlyCollection<AdoptedBookItemDto> Books) { }
}
