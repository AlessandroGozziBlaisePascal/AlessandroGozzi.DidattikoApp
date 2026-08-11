using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto
{
    public record SmartCartCalculationDto(
        decimal ItemsTotal, 
        decimal ShippingTotal, 
        decimal GrandTotal, 
        bool IsFreeShippingApplied, 
        decimal AmountNeededForFreeShipping,
        int UniqueVendorsCount,
        int VendorCount
    );
}
