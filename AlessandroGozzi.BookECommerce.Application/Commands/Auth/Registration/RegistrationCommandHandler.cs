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
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
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

            #region Validation of the command properties
            var nameResult = command.Name.ToNameDomain();
            if(nameResult.IsFailure)
            {
                return Result.Failure<CustomerDto>(new Error("Name", nameResult.Error.Description, ErrorType.Validation));
            }

            var surnameResult = command.Surname.ToSurnameDomain();
            if(surnameResult.IsFailure)
            {
                return Result.Failure<CustomerDto>(new Error("Surname", surnameResult.Error.Description, ErrorType.Validation));
            }

            var emailResult = command.Email.ToEmailDomain();
            if(emailResult.IsFailure)
            {
                return Result.Failure<CustomerDto>(new Error("Email", emailResult.Error.Description, ErrorType.Validation));
            }

            var addressResult = command.Address.ToAddressDomain();
            if(addressResult.IsFailure)
            {
                return Result.Failure<CustomerDto>(new Error("Address", addressResult.Error.Description, ErrorType.Validation));
            }

            var numberResult = command.PhoneNumber.ToNumberDomain();
            if(numberResult.IsFailure)
            {
                return Result.Failure<CustomerDto>(new Error("Phone number", numberResult.Error.Description, ErrorType.Validation));
            }
            #endregion

            var customerResult = Customer.Create(
                new AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers.ValueObjects.FullName(nameResult.Value, surnameResult.Value),
                emailResult.Value,
                addressResult.Value,
                numberResult.Value,
                passwordHash);
            
            if(customerResult.IsFailure)
            {
                return Result.Failure<CustomerDto>(new Error("Customer", "Customer failed to be created", ErrorType.Failure));
            }

            CustRepo.Add(customerResult.Value);

            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success(customerResult.Value.ToDto());
        }

    }
}
