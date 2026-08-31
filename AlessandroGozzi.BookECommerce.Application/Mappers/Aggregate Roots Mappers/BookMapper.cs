using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books;
namespace AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers
{
    public static class BookMapper
    {
        public static BookDto ToDto(this Book book)
        {
            return new BookDto(
                book.Id,
                book.ISBNCode.Value,
                book.Title,
                book.Status.ToDto(),
                book.MainPhoto.Value,
                book.Price.Amount,
                book.IsAvailable,
                book.SellerId
            );
        }
    }
}
