using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Buyer_POV.TopUpWallet.Action
{
    public record TopUpWalletActionCommand(
        Guid CustomerId,
        decimal Amount,
        string PaymentIntentId
        ): IRequest<Result>;
}
