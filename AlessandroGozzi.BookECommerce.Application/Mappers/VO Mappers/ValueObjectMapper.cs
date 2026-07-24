using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
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
        public static ISBN ToISBNDomain(this string value) => ISBN.Create(value).Value;
        public static string ToDto(this ISBN value) => value.ToString();
        //SUBJECT
        public static Subject ToSubjectDomain(this string value) => Subject.Create(value).Value;
        public static string ToDto(this Subject value) => value.ToString();
        //NAME
        public static Name ToNameDomain(this string value) => Name.Create(value).Value;
        public static string ToDto(this Name value) => value.ToString();
        //SURNAME
        public static Surname ToSurnameDomain(this string value) => Surname.Create(value).Value;
        public static string ToDto(this Surname value) => value.ToString();
        //EXPIRYDATE
        public static ExpiryDate ToExpiryDateDomain(this string value) => ExpiryDate.Create(value).Value;
        public static string ToDto(this ExpiryDate value) => value.ToString();
        //ADDRESS
        public static Address ToAddressDomain(this AddressDto addressDto) => Address.Create(addressDto.Street,addressDto.Cnumber, addressDto.City, addressDto.CAP).Value;
        public static AddressDto ToDto(this Address value) => new AddressDto(value.Street,value.CivicNumber,value.City,value.CAP);
        //EMAIL
        public static Email ToEmailDomain(this string value) => Email.Create(value).Value;
        public static string ToDto(this Email value) => value.ToString();
        //PHONE NUMBER 
        public static PhoneNumber ToNumberDomain(this string value) => PhoneNumber.Create(value).Value;
        public static string ToDto(this PhoneNumber value) => value.ToString();
        //TAX CODE
        public static TaxCode ToTaxCodeDomain(this string value) => TaxCode.Create(value).Value;
        public static string ToDto(this TaxCode value) => value.ToString();
        //BOOK REVIEW
        public static BookReview ToReviewDomain(Guid customerId, string comment, int rating) => BookReview.Create(customerId, rating, comment).Value;
        public static string ToDto(this BookReview value) => value.ToString();
        //MONEY
        public static Money ToMoneyDomain(this decimal price) => Money.Create(price).Value;
        public static decimal ToDto(this Money money) => money.Amount;
        //BOOK STATUS
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
       
    }
}
