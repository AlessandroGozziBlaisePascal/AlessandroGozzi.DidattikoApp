using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.Customer_Queries.GetOrdersByCustomerId
{
    public sealed record GetOrdersByCustomerIdQuery(Guid CustomerId) : IRequest<Result<IEnumerable<OrderDto>>>;
}
