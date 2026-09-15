using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers.ValueObjects;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;
using Stripe;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.PasswordReset.Request
{
    public class RequestPasswordResetCommandHandler: IRequestHandler<RequestPasswordResetCommand, Result> 
    {
        private readonly ICustomerRepository CustRepo;
        private readonly IOtpService _otpService;
        private readonly IEmailSender _emailSender;
        private readonly ISMSSender _SMSSender;

        public RequestPasswordResetCommandHandler(ICustomerRepository custRepo, IOtpService otpService, IEmailSender emailSender, ISMSSender sMSSender)
        {
            CustRepo = custRepo;
            _otpService = otpService;
            _emailSender = emailSender;
            _SMSSender = sMSSender;
        }

        public async Task<Result> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(request.Identifier))
                return Result.Failure(new Error("InvalidIdentifier", "The identifier cannot be empty or whitespace.", ErrorType.Validation));
            
            var emailResult = Email.Create(request.Identifier);
            var numberResult = PhoneNumber.Create(request.Identifier);

            if(emailResult.IsFailure && numberResult.IsFailure)
                return Result.Failure(new Error("InvalidIdentifier", "The identifier must be a valid email or phone number.", ErrorType.Validation));

            if (! await EmailExistenceChecker.IsValidAndDelivelableAsync(request.Identifier) && !PhoneNumberExistenceChecker.IsValidMobile(request.Identifier))
                return Result.Failure(new Error("InvalidIdentifier", "The identifier must be a valid email or phone number.", ErrorType.Validation));

            var customer = await CustRepo.GetByIdentifierAsync(request.Identifier, cancellationToken);
            if (customer == null)
            {
                return Result.Success();
            }

            var otp = await _otpService.GenerateAndSaveOtpAsync(request.Identifier, cancellationToken);

            if(emailResult.IsSuccess)
            {
                var emailBody = $"Your password reset code is: {otp}";
                await _emailSender.SendEmailAsync(customer.Email.Value, "Reset password - BookECommerce", emailBody, cancellationToken);
            }
            else if(numberResult.IsSuccess)
            {
                var numberBody = $"[BookECommerce] Il tuo codice per reimpostare la password è: {otp}";
                await _SMSSender.SendSmsAsync(customer.Number.Value, numberBody, cancellationToken);
            }

            return Result.Success();
        }
    }
}
