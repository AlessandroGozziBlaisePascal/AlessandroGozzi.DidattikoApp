using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;
using MediatR;
using Stripe;

namespace AlessandroGozzi.BookECommerce.Application.Commands.UpdateCustomerAddress
{
    public class UpdateCustomerAddressCommandHandler: IRequestHandler<UpdateCustomerAddressCommand, Result<CustomerDto>>
    {
        private readonly ICustomerRepository CustomerRepo;

        public UpdateCustomerAddressCommandHandler(ICustomerRepository customerRepo)
        {
            CustomerRepo = customerRepo;
        }

        public async Task<Result<CustomerDto>> Handle(UpdateCustomerAddressCommand command, CancellationToken token)
        {
            if(command.AddressDto == null)
            {
                return Result.Failure<CustomerDto>(new Error("Customer address","New address is null",ErrorType.Validation));
            }
            
            var customer = await CustomerRepo.GetByIdAsync(command.Id, token);

            if(customer == null)
            {
                return Result.Failure<CustomerDto>(new Error("Customer", "Customer not found", ErrorType.NotFound));
            }

            var newAddressResult = AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object.Address.Create(command.AddressDto.Street, command.AddressDto.Cnumber, command.AddressDto.City, command.AddressDto.CAP);
            if (newAddressResult.IsFailure)
            {
                return Result.Failure<CustomerDto>(new Error("New address", "Address failed to be created", ErrorType.Failure));
            }

            var result = customer.ChangeAddress(newAddressResult.Value);

            if (result.IsFailure)
            {
                return Result.Failure<CustomerDto>(new Error("New address", "Address failed to be changed", ErrorType.Failure));
            }

            await CustomerRepo.UpdateAsync(customer, token);

            return Result.Success(customer.ToDto());


        }
    }
}
