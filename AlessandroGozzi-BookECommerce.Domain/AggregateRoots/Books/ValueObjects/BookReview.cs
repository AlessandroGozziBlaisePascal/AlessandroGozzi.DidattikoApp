using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers.ValueObjects;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books.ValueObjects
{
    public record BookReview
    {
        public Guid CustomerId { get; init; }
        public FullName CustomerName { get; init; }
        public int Rating { get; init; }
        public DateTime CreatedAt { get; init; }

        private BookReview() { }
        private BookReview(Guid custId, FullName custName, int rating)
        {
            CustomerId = custId;
            CustomerName = custName;
            Rating = rating;
            CreatedAt = DateTime.Now;
        }

        public static Result<BookReview> Create(Guid custId, FullName custName, int rating)
        {
            if (custName == null)
                return Result.Failure<BookReview>(new Error("Customer name", "Customer name is null", ErrorType.Validation));
            if (rating < 1 || rating > 5)
            {
                return Result.Failure<BookReview>(new Error("Review.InvalidRating", "The rating must be between 1 and 5 stars.",ErrorType.Validation));
            }

            return Result.Success(new BookReview(custId, custName, rating));
        }

        public override string ToString() => $"{CustomerName} {Rating} {CreatedAt}";
    }

}
