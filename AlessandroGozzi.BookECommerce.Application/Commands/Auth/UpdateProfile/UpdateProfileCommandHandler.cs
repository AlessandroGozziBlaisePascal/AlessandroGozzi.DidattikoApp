using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.CustomersValue_Object;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.UpdateProfile
{
    public class UpdateProfileCommandHandler: IRequestHandler<UpdateProfileCommand,Result>
    {
        private readonly ICustomerRepository CustomerRepository;
        private readonly IUnitOfWork UnitOfWork;

        public UpdateProfileCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
        {
            CustomerRepository = customerRepository;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateProfileCommand command, CancellationToken token)
        {
            var customer = await CustomerRepository.GetByIdAsync(command.CustomerId, token);
            if(customer == null)
            {
                return Result.Failure(new Error("Customer","Customer not found",ErrorType.NotFound));
            }

            List<string> updatedFields = new();

            bool isFullNameChanged = command.Name != null && command.Surname != null;
            bool isEmailChanged = command.Email != null;
            bool isAddressChanged = command.Address != null;
            bool isNumberChanged = command.Number != null;

            if (isFullNameChanged)
            {
                var conversion = ValueObjectMapper.ToFullNameDomain(command.Name!, command.Surname!);
                if (conversion.IsFailure)
                    return Result.Failure(conversion.Error);
                var result = customer.ChangeName(conversion.Value);
                if (result.IsFailure)
                    return Result.Failure(result.Error);
                updatedFields.Add("Nome/Cognome");
            }
            if (isEmailChanged)
            {
                var conversion = command.Email!.ToEmailDomain();
                if (conversion.IsFailure)
                    return Result.Failure(conversion.Error);
                var result = customer.ChangeEmail(conversion.Value);
                if (result.IsFailure)
                    return Result.Failure(result.Error);
                updatedFields.Add("Email");
            }
            if (isAddressChanged)
            {
                var conversion = command.Address!.ToAddressDomain();
                if (conversion.IsFailure)
                    return Result.Failure(conversion.Error);
                var result = customer.ChangeAddress(conversion.Value);
                if (result.IsFailure)
                    return Result.Failure(result.Error);
                updatedFields.Add("Address");
            }
            if (isNumberChanged)
            {
                var conversion = command.Number!.ToNumberDomain();
                if (conversion.IsFailure)
                    return Result.Failure(conversion.Error);
                var result = customer.ChangeNumber(conversion.Value);
                if (result.IsFailure)
                    return Result.Failure(result.Error);
                updatedFields.Add("Number");
            }

            customer.ProfileUpdated(updatedFields);

            await UnitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
