using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.AddCreditCard
{
    public sealed record AddCreditCardCommand(
        Guid CustomerId,
        string CardNumber,
        string CardHolderName,
        string CardHolderSurname,
        string ExpiryDate
    ) : IRequest<Result<CreditCardDto>>;
}
