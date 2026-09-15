using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books.ValueObjects;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers.ValueObjects;
using AlessandroGozzi.BookECommerce.Domain.ValueObjects;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers
{
    public static class ValueObjectMapper
    {
        //ISBN
        public static Result<ISBN> ToISBNDomain(this string isbn) => ISBN.Create(isbn);
        //SUBJECT
        public static Result<Subject> ToSubjectDomain(this string isbn) => Subject.Create(isbn);
        //NAME
        public static Result<Name> ToNameDomain(this string name) => Name.Create(name);
        //SURNAME
        public static Result<Surname> ToSurnameDomain(this string surname) => Surname.Create(surname);
        //FULLNAME
        public static Result<FullName> ToFullNameDomain(string name, string surname)
        {
            return name.ToNameDomain().IsFailure || surname.ToSurnameDomain().IsFailure
                ? Result.Failure<FullName>(new Error("Fullname.ToDomain","Name or surname is incorrect",ErrorType.Validation))
                : Result.Success(new FullName(name.ToNameDomain().Value,surname.ToSurnameDomain().Value));
        }
        //EXPIRYDATE
        public static Result<ExpiryDate> ToExpiryDateDomain(this string expiryDate) => ExpiryDate.Create(expiryDate);
        public static string ToDto(this ExpiryDate expiryDate) => expiryDate.ToString();
        //ADDRESS
        public static Result<Address> ToAddressDomain(this AddressDto addressDto) => Address.Create(addressDto.Street,addressDto.Cnumber, addressDto.City, addressDto.CAP);
        public static AddressDto ToDto(this Address value) => new AddressDto(value.Street,value.CivicNumber,value.City,value.CAP);
        //EMAIL
        public static Result<Email> ToEmailDomain(this string mail) => Email.Create(mail);
        //PHONE NUMBER 
        public static Result<PhoneNumber> ToNumberDomain(this string phoneNumber) => PhoneNumber.Create(phoneNumber);
        //BOOK REVIEW
        public static Result<BookReview> ToReviewDomain(Guid customerId, string name, string surname, int rating)
        {
            var fullNameResult = ToFullNameDomain(name, surname);
            if (fullNameResult.IsFailure)
                return Result.Failure<BookReview>(fullNameResult.Error);
            return BookReview.Create(customerId, fullNameResult.Value, rating);
        } 
        public static string ToDto(this BookReview value) => value.ToString();
        //MONEY
        public static Result<Money> ToMoneyDomain(this decimal price) => Money.Create(price);
        //BOOK STATUS
        public static Result<BookStatus> ToBookStatusDomain(this string status)
        {
            if(Enum.TryParse<BookStatus>(status, ignoreCase: true, out var result))
                return Result.Success(result);

            return Result.Failure<BookStatus>(new Error("Book status", "Invalid book status", ErrorType.Validation));
        }
        public static string ToDto(this BookStatus status) => status.ToString();
        //IMAGE URL
        public static Result<ImageUrl> ToImageUrlDomain(this string? url)
        {
            var imageResult = ImageUrl.Create(url);
            if(imageResult.IsFailure)
                return Result.Failure<ImageUrl>(imageResult.Error);
            return Result.Success(imageResult.Value);
        }
        //IBAN
        public static Result<IBAN> ToIbanDomain(this string iban) => IBAN.Create(iban);
    }
}
