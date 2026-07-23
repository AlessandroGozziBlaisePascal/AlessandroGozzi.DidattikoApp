using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers
{
    public static class BookMapper
    {
        public static BookDto ToDto(this Book book, Customer seller)
        {
            return new BookDto(
                book.Id,
                book.ISBNCode.ToDto(),
                book.Title,
                book.Status.ToDto(),
                book.MainPhoto,
                book.Price.Amount,
                book.IsAvailable,
                seller.Id,
                $"{seller.Name.Value} {seller.Surname.Value}"
            );
        }
    }
}
