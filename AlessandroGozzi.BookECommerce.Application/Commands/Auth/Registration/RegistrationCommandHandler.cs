using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.Registration
{
    public class RegistrationCommandHandler: IRequestHandler<RegistrationCommand, Result<CustomerDto>>
    {
        private readonly ICustomerRepository CustRepo;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IPasswordHasher _passHasher;

        public RegistrationCommandHandler(ICustomerRepository custRepo, IUnitOfWork unitOfWork, IPasswordHasher passHasher)
        {
            CustRepo = custRepo;
            UnitOfWork = unitOfWork;
            _passHasher = passHasher;
        }

        public async Task<Result<CustomerDto>> Handle(RegistrationCommand command, CancellationToken token)
        {
            if(command.Password != command.ConfirmPassword)
            {
                return Result.Failure<CustomerDto>(new Error("Password", "Passwords are not the same", ErrorType.Validation));
            }
            string passwordHash = _passHasher.HashPassword(command.Password);

            if(await CustRepo.GetByIdentifierAsync(command.Email, token) != null)
            {
                return Result.Failure<CustomerDto>(new Error("New customer credential", "Email already in using", ErrorType.Validation));
            }
            if (await CustRepo.GetByIdentifierAsync(command.PhoneNumber, token) != null)
            {
                return Result.Failure<CustomerDto>(new Error("New customer credential", "Number already in using", ErrorType.Validation));
            }

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

            await CustRepo.AddAsync(customerResult.Value, token);

            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success(customerResult.Value.ToDto());
        }

    }
}
