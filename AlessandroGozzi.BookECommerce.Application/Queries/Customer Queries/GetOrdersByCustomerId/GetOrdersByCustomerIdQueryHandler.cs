using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.Customer_Queries.GetOrdersByCustomerId
{
    public class GetOrdersByCustomerIdQueryHandler: IRequestHandler<GetOrdersByCustomerIdQuery, Result<IEnumerable<OrderDto>>>
    {
        private readonly IOrderRepository Repo;

        public GetOrdersByCustomerIdQueryHandler(IOrderRepository repo)
        {
            Repo = repo;
        }

        public async Task<Result<IEnumerable<OrderDto>>> Handle(GetOrdersByCustomerIdQuery request, CancellationToken token)
        {
            var orders = await Repo.GetAllByCustomerIdAsync(request.CustomerId, token);

            var dtos = orders.Select(o => o.ToDto());

            return Result.Success(dtos);
        }
    }
}
