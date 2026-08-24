using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments.ValueObjects;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.StartCheckoutPayment
{
    public sealed record StartCheckoutPaymentCommand(Guid CustomerId, ShippingType Type): IRequest<Result<CheckoutPaymentResultDto>>;
}
