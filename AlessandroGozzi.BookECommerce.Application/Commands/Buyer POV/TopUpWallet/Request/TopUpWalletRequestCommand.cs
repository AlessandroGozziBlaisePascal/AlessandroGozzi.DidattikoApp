using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Buyer_POV.Request.TopUpWallet
{
    public record TopUpWalletRequestCommand(
        Guid CustomerId,
        decimal Amount
        ): IRequest<Result>;
}
