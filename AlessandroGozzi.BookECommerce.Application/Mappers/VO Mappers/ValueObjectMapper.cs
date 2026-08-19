using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers
{
    public static class ValueObjectMapper
    {
        //ISBN
        public static Result<ISBN> ToISBNDomain(this string value) => ISBN.Create(value);
        public static string ToDto(this ISBN value) => value.ToString();
        //SUBJECT
        public static Result<Subject> ToSubjectDomain(this string value) => Subject.Create(value);
        public static string ToDto(this Subject value) => value.ToString();
        //NAME
        public static Result<Name> ToNameDomain(this string value) => Name.Create(value);
        public static string ToDto(this Name value) => value.ToString();
        //SURNAME
        public static Result<Surname> ToSurnameDomain(this string value) => Surname.Create(value);
        public static string ToDto(this Surname value) => value.ToString();
        //FULLNAME
        public static FullName ToFullNameDomain(string name, string surname)
        {

        }
        //EXPIRYDATE
        public static Result<ExpiryDate> ToExpiryDateDomain(this string value) => ExpiryDate.Create(value);
        public static string ToDto(this ExpiryDate value) => value.ToString();
        //ADDRESS
        public static Result<Address> ToAddressDomain(this AddressDto addressDto) => Address.Create(addressDto.Street,addressDto.Cnumber, addressDto.City, addressDto.CAP);
        public static AddressDto ToDto(this Address value) => new AddressDto(value.Street,value.CivicNumber,value.City,value.CAP);
        //EMAIL
        public static Result<Email> ToEmailDomain(this string value) => Email.Create(value);
        public static string ToDto(this Email value) => value.ToString();
        //PHONE NUMBER 
        public static Result<PhoneNumber> ToNumberDomain(this string value) => PhoneNumber.Create(value);
        public static string ToDto(this PhoneNumber value) => value.ToString();
        //TAX CODE
        public static Result<TaxCode> ToTaxCodeDomain(this string value) => TaxCode.Create(value);
        public static string ToDto(this TaxCode value) => value.ToString();
        //BOOK REVIEW
        public static Result<BookReview> ToReviewDomain(Guid customerId,  int rating) => BookReview.Create(customerId, rating);
        public static string ToDto(this BookReview value) => value.ToString();
        //MONEY
        public static Result<Money> ToMoneyDomain(this decimal price) => Money.Create(price);
        public static decimal ToDto(this Money money) => money.Amount;
        //BOOK STATUS
        public static Result<BookStatus> ToDomain(this string status)
        {
            if(Enum.TryParse<BookStatus>(status, ignoreCase: true, out var result))
                return Result.Success(result);

            return Result.Failure<BookStatus>(new Error("Book status", "Invalid book status", ErrorType.Validation));
        }
        public static string ToDto(this BookStatus status) => status.ToString();
       
    }
}
