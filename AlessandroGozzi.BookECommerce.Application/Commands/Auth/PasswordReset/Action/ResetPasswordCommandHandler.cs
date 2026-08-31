using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.PasswordReset.Action
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly ICustomerRepository CustRepo;
        private readonly IOtpService _otpService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork UnitOfWork;

        public ResetPasswordCommandHandler(ICustomerRepository custRepo, IOtpService otpService, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            CustRepo = custRepo;
            _otpService = otpService;
            _passwordHasher = passwordHasher;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return Result.Failure(new Error("ResetPassword.Mismatch", "La nuova password e la conferma non coincidono.", ErrorType.Validation));
            }

            var customer = await CustRepo.GetByIdentifierAsync(request.Identifier, cancellationToken);
            if (customer is null)
            {
                return Result.Failure(new Error("ResetPassword.UserNotFound", "Utente non trovato.", ErrorType.NotFound));
            }

            // 3. Validazione codice OTP inserito dal customer (verifica validità e scadenza)
            bool isValidCode = await _otpService.ValidateOtpAsync(request.Identifier, request.Code, cancellationToken);
            if (!isValidCode)
            {
                return Result.Failure(new Error("ResetPassword.InvalidCode", "Il codice di verifica è errato o scaduto.", ErrorType.Validation));
            }

            string newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);

            customer.ChangePassword(newPasswordHash);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await _otpService.InvalidateOtpAsync(request.Identifier, request.Code, cancellationToken);

            return Result.Success();
        }
    }
}
