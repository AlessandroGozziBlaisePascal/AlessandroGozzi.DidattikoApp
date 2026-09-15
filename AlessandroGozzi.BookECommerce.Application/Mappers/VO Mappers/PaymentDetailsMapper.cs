using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Orders.ValueObjects;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers
{
    public static class PaymentDetailsMapper
    {
        public static PaymentDetailsDto ToDto(this PaymentDetails details) => new PaymentDetailsDto(details.PaymentMethod.ToString(), details.Description);
    }
}
