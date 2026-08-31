using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Wallets;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers
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
