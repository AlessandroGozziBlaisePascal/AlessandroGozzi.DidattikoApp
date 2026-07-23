using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Checkout
{
    public record ShippingAddressDto(string Street, string CivicNumber, string City, string CAP)
    {
        public override string ToString() => $"{Street} {CivicNumber}, {City} {CAP}";
    }
}
