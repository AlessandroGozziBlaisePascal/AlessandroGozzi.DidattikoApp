using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Shipments.ValueObjects;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.CompleteCheckoutPayment
{
    public record CompleteCheckoutPaymentCommand(
        Guid CustomerId,
        Guid CartId,
        string PaymentIntentId,
        ShippingType Type
    ) : IRequest<Result<OrderDto>>;
}
