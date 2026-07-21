using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Value_Object
{
    public record BookReview
    {
        public Guid CustomerId { get; init; }
        public string Text { get; init; }
        public int Rating { get; init; }
        public DateTime CreatedAt { get; init; }
        private BookReview(Guid customerId, string text, int rating, DateTime createdAt)
        {
            CustomerId = customerId;
            Text = text;
            Rating = rating;
            CreatedAt = createdAt;
        }

        public static Result<BookReview> Create(Guid customerId, int rating, string text)
        {
            if (customerId == Guid.Empty)
            {
                return Result.Failure<BookReview>(new Error("Review.InvalidCustomer","A valid Customer ID must be provided.", ErrorType.Validation));
            }
            if (rating < 1 || rating > 5)
            {
                return Result.Failure<BookReview>(new Error("Review.InvalidRating", "The rating must be between 1 and 5 stars.",ErrorType.Validation));
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                return Result.Failure<BookReview>(new Error("Review.EmptyText", "Comment text cannot be empty.",ErrorType.Validation));
            }

            return Result.Success(new BookReview(customerId, text.Trim(), rating, DateTime.UtcNow));
        }
    }

}
