using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.AddToCart
{
    public sealed record AddToCartCommand(Guid CustomerId, Guid BookId, int quantity): IRequest<Result>;
}
