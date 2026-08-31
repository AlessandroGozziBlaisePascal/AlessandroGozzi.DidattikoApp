using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto
{
    public record WalletDto(
        decimal AvailableBalance,
        decimal PendingBalance,
        decimal TotalBalance
        );

}
