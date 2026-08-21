using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Value_Object;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.Cart_Queries.GetCartSummary
{
    public record GetCartSummaryQuery(Guid CartId, ShippingType Type): IRequest<Result<CartSummaryDto>>;
}
