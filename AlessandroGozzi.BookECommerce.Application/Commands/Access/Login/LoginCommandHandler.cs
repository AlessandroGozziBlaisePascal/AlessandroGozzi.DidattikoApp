using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Access;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Login
{
    public class LoginCommandHandler: IRequestHandler<LoginCommand, Result<LoginProfileResponseDto>>
    private readonly ICustomerRepository CustRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly ICreditCardRepository _creditCardRepository;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        ICreditCardRepository creditCardRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _creditCardRepository = creditCardRepository;
    }

    public async Task<Result<LoginProfileResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var identifier = request.Identifier.Trim();

        // 1. Cerca l'utente sia per Email che per Numero di Telefono
        User? user;
        if (IsEmail(identifier))
        {
            user = await _userRepository.GetByEmailAsync(identifier, cancellationToken);
        }
        else
        {
            // Se è un numero di telefono, è buona norma formattarlo o pulirlo prima della ricerca
            user = await _userRepository.GetByPhoneNumberAsync(identifier, cancellationToken);
        }

        if (user is null)
        {
            return Result.Failure<LoginProfileResponseDto>(
                new Error("Auth.InvalidCredentials", "Credenziali non valide.", ErrorType.Validation));
        }

        // 2. Verifica la Password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<LoginProfileResponseDto>(
                new Error("Auth.InvalidCredentials", "Credenziali non valide.", ErrorType.Validation));
        }

        // 3. Genera Token e prepara il DTO di risposta
        var token = _jwtProvider.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(2);
        var savedCard = await _creditCardRepository.GetDefaultCardByUserIdAsync(user.Id, cancellationToken);

        var response = new LoginProfileResponseDto(
            AccessToken: token,
            ExpiresAt: expiresAt,
            RefreshToken: null,
            Customer: new CustomerDto(user.Id, user.FirstName, user.LastName, user.Email),
            HasSavedCreditCard: savedCard is not null,
            MaskedCardNumber: savedCard?.MaskedNumber
        );

        return Result.Success(response);
    }

    // Helper method per verificare se l'identifier è un'email
    private static bool IsEmail(string input) => input.Contains('@');
}
