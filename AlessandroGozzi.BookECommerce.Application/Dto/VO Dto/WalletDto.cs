using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities;

namespace AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto
{
    public record WalletDto(
        decimal AvailableBalance,
        decimal PendingBalance,
        decimal TotalBalance
        );

}
