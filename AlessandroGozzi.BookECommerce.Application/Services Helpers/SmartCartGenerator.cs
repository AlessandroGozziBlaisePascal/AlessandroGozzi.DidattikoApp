using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Orders;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Shipments.ValueObjects;

namespace AlessandroGozzi.BookECommerce.Application.Services_Helpers
{
    public class SmartCartGenerator
    {
        private const decimal FreeShippingThreshold = 30.00m;

        public static SmartCartCalculationDto Calculate(List<CartItemDto> cartItems, ShippingType type)
        {
            decimal itemsTotalPrice = cartItems.Sum(item => item.UnitPrice * item.Quantity);

            int uniqueVendorsCount = cartItems
                .Select(item => item.SellerId)
                .Distinct()
                .Count();

            if (itemsTotalPrice >= FreeShippingThreshold)
            {
                return new SmartCartCalculationDto(
                itemsTotalPrice,
                0,
                itemsTotalPrice,
                false,
                0,
                uniqueVendorsCount
                );
            }
            decimal totalShippingFee = uniqueVendorsCount * Order.ShippingCosts[type];
            decimal grandTotal = itemsTotalPrice + totalShippingFee;

            return new SmartCartCalculationDto(
                itemsTotalPrice,
                totalShippingFee,
                grandTotal,
                false,
                FreeShippingThreshold - itemsTotalPrice,
                uniqueVendorsCount
                );
        }
    }
}