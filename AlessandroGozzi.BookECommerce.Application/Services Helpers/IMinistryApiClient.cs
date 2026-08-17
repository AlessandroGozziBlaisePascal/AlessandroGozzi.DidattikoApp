using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.BookSearching;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi.BookECommerce.Application
{
    public interface IMinistryApiClient
    {
        Task<Result<ClassAdoptionResponseDto>> GetAdoptedBooksAsync(string schoolCode, int grade, string section, string academicYear);
    }
}
