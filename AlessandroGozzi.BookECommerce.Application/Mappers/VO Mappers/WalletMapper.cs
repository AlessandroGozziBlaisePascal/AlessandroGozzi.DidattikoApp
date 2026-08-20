using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers
{
    public static class WalletMapper
    {
        public static WalletDto ToDto(this Wallet wallet)
        {
            return new WalletDto(
                wallet.AvailableBalance.Amount,
                wallet.PendingBalance.Amount,
                wallet.TotalBalance.Amount
                );
        }
    }
}
