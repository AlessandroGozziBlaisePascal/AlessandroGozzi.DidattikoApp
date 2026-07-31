using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.RegisterNewCustomer
{
    public class RegisterNewCustomerCommandHandler: IRequestHandler<RegisterNewCustomerCommand, Result<CustomerDto>>
    {
        private readonly ICustomerRepository CustRepo;
        private readonly IUnitOfWork UnitOfWork;

        public RegisterNewCustomerCommandHandler(ICustomerRepository custRepo, IUnitOfWork unitOfWork) 
        {
            CustRepo = custRepo;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result<CustomerDto>> Handle(RegisterNewCustomerCommand command, CancellationToken token)
        {
            if(command.Password != command.ConfirmPassword)
            {
                return Result.Failure<CustomerDto>(new Error("Password", "Passwords are not the same", ErrorType.Validation));
            }
            string passwordHash = _passHasher.HashPassword(); //TODO: Implement IPasswordHasher

            var customerResult = Customer.Create(
                command.Name.ToNameDomain(),
                command.Surname.ToSurnameDomain(),
                command.Email.ToEmailDomain(),
                command.Address.ToAddressDomain(),
                command.PhoneNumber.ToNumberDomain(),
                command.TaxCode.ToTaxCodeDomain(),
                passwordHash);
            
            if(customerResult.IsFailure)
            {
                return Result.Failure<CustomerDto>(new Error("Customer", "Customer failed to be created", ErrorType.Failure));
            }

            await UnitOfWork.SaveChangesAsync();

            return Result.Success(customerResult.Value.ToDto());
        }

    }
}
