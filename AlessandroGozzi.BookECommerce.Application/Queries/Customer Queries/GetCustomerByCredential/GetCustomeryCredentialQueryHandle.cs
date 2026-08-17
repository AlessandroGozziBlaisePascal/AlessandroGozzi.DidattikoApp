using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.Customer_Queries.GetCustomerByCredential
{
    public sealed class GetCustomerByCredentialQueryHandle : IRequestHandler<GetCustomerByCredentialQuery, Result<CustomerDto>>
    {
        private readonly ICustomerRepository Repo;
        private readonly IAuthenticationService AuthService;

        public GetCustomerByCredentialQueryHandle(ICustomerRepository repo, IAuthenticationService service)
        {
            Repo = repo;
            AuthService = service;
        }

        public async Task<Result<CustomerDto>> Handle(GetCustomerByCredentialQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Identifier))
            {
                return Result.Failure<CustomerDto>(new Error("Customer identifier credential", "Email or phone number is null", ErrorType.Validation));
            }
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return Result.Failure<CustomerDto>(new Error("Customer identifier credential", "Password is null", ErrorType.Validation));
            }

            var customer = await Repo.GetByIdentifierAsync(request.Identifier.Trim(), cancellationToken);

            if(customer == null)
            {
                return Result.Failure<CustomerDto>(new Error("Customer","Customer not found with this credential", ErrorType.NotFound));
            }

            bool isPasswordValid = AuthService.VerifyPassword(customer.Id, request.Password, cancellationToken).Result;
            if (!isPasswordValid)
            {
                return Result.Failure<CustomerDto>(new Error("Access password", "Incorrect password", ErrorType.Validation));
            }
            return Result.Success(customer.ToDto());
        }
    }

}