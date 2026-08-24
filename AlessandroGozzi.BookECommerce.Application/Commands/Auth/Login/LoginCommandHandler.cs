using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Auth;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers.ValueObjects;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using MediatR;
using Stripe;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.Login
{
    public class LoginCommandHandler: IRequestHandler<LoginCommand, Result<LoginResponseDto>>
    {
        private readonly ICustomerRepository CustRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public LoginCommandHandler(ICustomerRepository custRepo, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            CustRepo = custRepo;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var identifier = request.Identifier.Trim();

            if (IsEmail(identifier))
            {
                var identifierResult = Email.Create(identifier);
                if (identifierResult.IsFailure)
                    return Result.Failure<LoginResponseDto>(new Error("Email", "Invalid email format", ErrorType.Validation));
            }
            else
            {
                var identifierResult = PhoneNumber.Create(identifier);
                if (identifierResult.IsFailure)
                    return Result.Failure<LoginResponseDto>(new Error("Phone number", "Invalid number format", ErrorType.Validation));
            }
            
            var customer = await CustRepo.GetByIdentifierAsync(identifier, cancellationToken);

            if (customer is null)
            {
                return Result.Failure<LoginResponseDto>(new Error("Auth.InvalidCredentials", "Credenziali non valide.", ErrorType.Validation));
            }

            if (!_passwordHasher.VerifyPassword(request.Password, customer.PasswordHash))
            {
                return Result.Failure<LoginResponseDto>(
                    new Error("Auth.InvalidCredentials", "Credenziali non valide.", ErrorType.Validation));
            }

            // 3. Genera Token e prepara il DTO di risposta
            var token = _jwtProvider.GenerateToken(customer.ToDto());
            var expiresAt = DateTime.UtcNow.AddHours(2);
            var savedCard = customer.CreditCard;

            var response = new LoginResponseDto(
                AccessToken: token,
                ExpiresAt: expiresAt,
                RefreshToken: null,
                Customer: new CustomerDto(customer.Id, customer.Name.ToDto(), customer.Surname.ToDto(), customer.Email.ToDto(), customer.Number.ToDto(), customer.Address.ToDto(), customer.TaxCode.ToDto()),
                HasSavedCreditCard: savedCard is not null,
                MaskedCardNumbers: savedCard?.DisplayName
            );

            return Result.Success(response);
        }

        private static bool IsEmail(string input) => input.Contains('@');
    }
}
