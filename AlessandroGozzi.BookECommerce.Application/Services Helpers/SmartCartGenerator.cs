using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Value_Object;

namespace AlessandroGozzi.BookECommerce.Application.Services_Helpers
{
    public class SmartCartGenerator
    {
        private const decimal FreeShippingThreshold = 30.00m;

        public static SmartCartCalculationResultDto Calculate(List<CartItemDto> cartItems, ShippingType type)
        {
            decimal itemsTotalPrice = cartItems.Sum(item => item.UnitPrice * item.Quantity);

            int uniqueVendorsCount = cartItems
                .Select(item => item.SellerId)
                .Distinct()
                .Count();

            if (itemsTotalPrice >= FreeShippingThreshold)
            {
                return new SmartCartCalculationResultDto(
                itemsTotalPrice,
                0,
                itemsTotalPrice,
                false,
                uniqueVendorsCount
                );
            }
            decimal totalShippingFee = uniqueVendorsCount * Order.ShippingCosts[type];
            decimal grandTotal = itemsTotalPrice + totalShippingFee;

            return new SmartCartCalculationResultDto(
                itemsTotalPrice,
                totalShippingFee,
                grandTotal,
                false,
                uniqueVendorsCount
                );
        }
    }
}