using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.Customer_Queries.GetCustomerByCredential
{
    public sealed record GetCustomerByCredentialQuery(string Identifier, string Password): IRequest<Result<CustomerDto>>;
}
