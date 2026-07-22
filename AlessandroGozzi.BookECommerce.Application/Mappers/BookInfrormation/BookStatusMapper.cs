using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.BookInfrormation
{
    public static class BookStatusMapper
    {
        public static string ToDto(this BookStatus status)
        {
            return status switch
            {
                BookStatus.LikeNew => "LikeNew",
                BookStatus.Highlighted => "Highlighted",
                BookStatus.PencilMarked => "PencilMarked",
                BookStatus.PenMarked => "PenMarked",
                BookStatus.Worn => "Worn",
                BookStatus.TornPages => "TornPages",
                _ => throw new ArgumentOutOfRangeException("Invalid status")
            };
        }

        public static BookStatus ToDomain(this string status)
        {
            return status switch
            {
                "LikeNew" => BookStatus.LikeNew,
                "Highlighted" => BookStatus.Highlighted,
                "PencilMarked" => BookStatus.PencilMarked,
                "PenMarked" => BookStatus.PenMarked,
                "Worn" => BookStatus.Worn,
                "TornPages" => BookStatus.TornPages,
                _ => throw new ArgumentException("Invalid status string")
            };
        }
    }
}
