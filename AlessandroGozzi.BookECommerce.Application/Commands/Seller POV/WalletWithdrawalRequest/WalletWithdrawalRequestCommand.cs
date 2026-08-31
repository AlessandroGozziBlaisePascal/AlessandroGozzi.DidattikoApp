using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Seller_POV.WalletWithdrawalRequest
{
    public record WalletWithdrawalRequestCommand(
        Guid CustomerId,
        decimal Amount,
        string Iban
        ): IRequest<Result>;
}
