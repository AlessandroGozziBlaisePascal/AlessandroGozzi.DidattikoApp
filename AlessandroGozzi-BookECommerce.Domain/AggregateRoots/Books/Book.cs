using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books.Events;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books.ValueObjects;
using AlessandroGozzi.BookECommerce.Domain.ValueObjects;

namespace AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books
{
    public class Book: AggregateRoot
    {
        public string Title { get; init; }
        public Guid SellerId { get; init; }
        public ISBN ISBNCode { get; init; }
        public Subject Subject { get; init; }
        public int SchoolYear { get; init; }
        public int PublicationYear { get; init; }
        public Money Price { get; private set; }
        public BookStatus Status { get; private set; }
        public ImageUrl MainPhoto { get; private set;}
        public double AverageRating { get; private set; } 
        public int RatingsNumber { get; private set; }
        public bool IsAvailable { get; private set; }

        private int TotalRating = 0;
        public List<BookReview> Reviews { get; private set; }

        private Book(string title, Guid sellerId, Subject sbj, ISBN code, int schoolYear, int publYear, Money price, BookStatus bookStatus, ImageUrl mainPhoto)
        {
            Title = title;
            SellerId = sellerId;
            ISBNCode = code;
            Subject = sbj;
            SchoolYear = schoolYear;
            PublicationYear = publYear;
            Price = price;
            Status = bookStatus;
            Reviews = new();
            IsAvailable = true;
            MainPhoto = mainPhoto;
        }

        private Book() { }

        public static Result<Book> Create(string title, Guid sellerId, Subject sbj, ISBN code, int schoolYear, int publYear, Money price, BookStatus bookStatus, ImageUrl mainPhoto)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Result.Failure<Book>(new Error("Title", "Title cannot be null", ErrorType.Validation));

            if (sbj == null)
                return Result.Failure<Book>(new Error("Subject", "Subject cannot be null", ErrorType.Validation));

            if (code == null)
                return Result.Failure<Book>(new Error("ISBN code", "ISBN cannot be null", ErrorType.Validation));

            if (schoolYear < 1 || schoolYear > 5)
                return Result.Failure<Book>(new Error("School book year", "School year of book must be between 1 and 5", ErrorType.Validation));

            if (publYear < 2000 || publYear > DateTime.UtcNow.Year)
                return Result.Failure<Book>(new Error("Publication book year", "Book publication must be between 2000 and current year", ErrorType.Validation));

            if (price == null)
                return Result.Failure<Book>(new Error("Price", "Book price can't be null", ErrorType.Validation));
            if(mainPhoto == null)
                return Result.Failure<Book>(new Error("Photo", "Book photo can't be null", ErrorType.Validation));
            return Result.Success(new Book(title, sellerId, sbj, code, schoolYear, publYear, price, bookStatus, mainPhoto));
        }

        public Result AddReview(BookReview r)
        {
            if (Reviews.Any(c => c.CustomerId == r.CustomerId))
            {
                return Result.Failure(new Error("Review.Duplicate", "This customer has already reviewed this book.", ErrorType.StatusConflict));
            }

            if (SellerId == r.CustomerId)
            {
                return Result.Failure(new Error("Review.SelfReview", "Book seller cannot add self review", ErrorType.StatusConflict));
            }

            Reviews.Add(r);

            RatingsNumber++;
            TotalRating += r.Rating;
            AverageRating = (double)TotalRating / RatingsNumber;

            Raise(new ReviewAddedEvent(Id, r));
            return Result.Success();
        }

        public Result RemoveReview(Guid customerId)
        {
            var existingReview = Reviews.FirstOrDefault(c => c.CustomerId == customerId);

            if (existingReview is null)
            {
                return Result.Failure(new Error("Review", "No review found for this customer on this book.", ErrorType.NotFound));
            }

            Reviews.Remove(existingReview);

            RatingsNumber--;
            TotalRating -= existingReview.Rating;

            AverageRating = RatingsNumber > 0 ? (double)TotalRating / RatingsNumber : 0.0;

            Raise(new ReviewRemovedEvent(Id, existingReview));
            return Result.Success();
        }

        public Result ChangePhoto(Guid requesterCustomerId, ImageUrl imgUrl)
        {
            if (SellerId != requesterCustomerId)
                return Result.Failure(new Error("New photo", "Cannot update photo. You are not the seller", ErrorType.PermissionDenied));
            if(imgUrl == null)
                return Result.Failure(new Error("New photo", "Cannot update null photo", ErrorType.Validation));
            MainPhoto = imgUrl;
            Raise(new PhotoChangedEvent(Id, MainPhoto));
            return Result.Success();
        }

        public Result UpdatePrice(Money price, Guid requesterCustomer)
        {
            if (SellerId != requesterCustomer)
                return Result.Failure(new Error("New price", "Cannot update price. You are not the seller", ErrorType.PermissionDenied));

            if (price == null)
                return Result.Failure(new Error("New price", "Cannot update to a null price", ErrorType.Validation));

            if (Price == price)
                return Result.Success();

            var p = Price;
            Price = price;
            Raise(new PriceUpdatedEvent(Id, p, Price));
            return Result.Success();
        }

        public Result UpdateStatus(BookStatus status, Guid requesterCustomer)
        {
            if (SellerId != requesterCustomer)
                return Result.Failure(new Error("New status", "Cannot update status. You are not the seller", ErrorType.PermissionDenied));

            if (Status == status)
                return Result.Success();

            var s = Status;
            Status = status;
            Raise(new StatusUpdatedEvent(Id, s, Status));
            return Result.Success();
        }

        public Result RemoveFromMarket(Guid requesterCustomer)
        {
            if (SellerId != requesterCustomer)
                return Result.Failure(new Error("New status", "Cannot update status. You are not the seller", ErrorType.PermissionDenied));

            if (!IsAvailable)
                return Result.Failure(new Error("Book.Remove", "Book is already removed from market", ErrorType.StatusConflict));

            IsAvailable = false;
            Raise(new BookRemovedFromMarketEvent(Id));
            return Result.Success();
        }

        public Result RestoreInMarket(Guid requesterCustomer)
        {
            if (SellerId != requesterCustomer)
                return Result.Failure(new Error("New status", "Cannot update status. You are not the seller", ErrorType.PermissionDenied));

            if (IsAvailable)
                return Result.Failure(new Error("Book.Restore", "Book is already restored in market", ErrorType.StatusConflict));

            IsAvailable = true;
            Raise(new BookRestoredInMarketEvent(Id));
            return Result.Success();
        }

        internal void RestoreAvailability() => IsAvailable = true;
    }
}