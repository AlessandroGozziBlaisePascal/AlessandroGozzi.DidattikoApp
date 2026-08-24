using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly ICustomerRepository CustomerRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork UnitOfWork;

        public ChangePasswordCommandHandler(ICustomerRepository customerRepo, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            CustomerRepo = customerRepo;
            _passwordHasher = passwordHasher;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken token)
        {
            var customer = await CustomerRepo.GetByIdAsync(command.CustomerId,token);
            if(customer == null)
            {
                return Result.Failure(new Error("Customer", "Customer not found", ErrorType.NotFound));
            }

            bool isPasswordCorrect = _passwordHasher.VerifyPassword(command.CurrentPassword, customer.PasswordHash);
            if (!isPasswordCorrect)
                return Result.Failure(new Error("Password", "Incorrect password", ErrorType.PermissionDenied));

            var result = customer.ChangePassword(_passwordHasher.HashPassword(command.NewPassword));
            if(result.IsFailure)
                return Result.Failure(result.Error);

            await UnitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
