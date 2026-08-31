using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books.ValueObjects;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers
{
    public static class ReviewMapper
    {
        public static ReviewDto ToDto(this BookReview review) => new ReviewDto(
            review.CustomerId,
            review.Rating,
            review.CustomerName.Name.Value,
            review.CustomerName.Surname.Value,
            review.CreatedAt
            );
    }
}
