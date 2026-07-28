using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.RegisterNewCustomer
{
    public class RegisterNewCustomerCommandHandler: IRequestHandler<RegisterNewCustomerCommand, Result<CustomerDto>
    {
        private readonly ICustomerRepository CustRepo;

        public RegisterNewCustomerCommandHandler(ICustomerRepository custRepo) 
        {
            CustRepo = custRepo;
        }

        public async Task<Result<CustomerDto>> Handle(RegisterNewCustomerCommand command, CancellationToken token)
        {
            if(command.Password != command.ConfirmPassword)
            {
                return Result.Failure<CustomerDto>(new Error("Password", "Passwords are not the same", ErrorType.Validation));
            }
            string passwordHash = _passHasher.HashPassword() //TODO: Implement IPasswordHasher

            var customerResult = Customer.Create() // TODO: Add password in Customer Domain
        }

    }
}
