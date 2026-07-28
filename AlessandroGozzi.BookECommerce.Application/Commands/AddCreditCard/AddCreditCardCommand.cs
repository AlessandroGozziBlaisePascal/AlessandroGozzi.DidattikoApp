using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.AddCreditCard
{
    public sealed record AddCreditCardCommand(
        Guid CustomerId,
        string CardNumber,
        string CardHolderName,
        string CardHolderSurname,
        string ExpiryDate,
        string Cvv
    ) : IRequest<Result>;
}
