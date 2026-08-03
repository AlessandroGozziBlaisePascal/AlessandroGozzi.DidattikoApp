using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.BookSearching
{
    public record SchoolAdoptionSearchRequestDto(string SchoolCode, int ClassYear, string Section, string AcademicYear) {}
}
