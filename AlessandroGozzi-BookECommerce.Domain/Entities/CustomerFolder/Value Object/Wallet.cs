using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object
{
    public class Wallet
    {
        public Money AvailableBalance { get; private set; }
        public Money PendingBalance { get; private set; }
        public Money TotalBalance => AvailableBalance + PendingBalance;

        public Wallet()
        {
            AvailableBalance = Money.Create(0m).Value;   
            PendingBalance = Money.Create(0m).Value;
        }

        public Result Deposit(Money money)
        {
            if (money.Amount == 0)
            {
                return Result.Failure(new Error("Wallet.Deposit", "Deposit amount must be greater than zero.", ErrorType.Validation));
            }
            AvailableBalance += money;
            return Result.Success();
        }

        public Result Withdraw(Money money)
        {
            if (money.Amount > AvailableBalance.Amount)
            {
                return Result.Failure(new Error("Wallet.Withdraw", "Insufficient balance for withdrawal.", ErrorType.Validation));
            }
            AvailableBalance -= money;
            return Result.Success();
        }

        public Result AddPendingFunds(Money money)
        {
            if (money.Amount == 0)
            {
                return Result.Failure(new Error("Wallet.AddPendingBalance", "Amount must be greater than zero.", ErrorType.Validation));
            }
            PendingBalance += money;
            return Result.Success();
        }

        public Result ReleasePendingFunds(Money money)
        {
            if (money.Amount == 0)
            {
                return Result.Failure(new Error("Wallet.ReleasePendingFunds", "Amount must be greater than zero.", ErrorType.Validation));
            }
            if (money.Amount > PendingBalance.Amount)
            {
                return Result.Failure(new Error("Wallet.ReleasePendingFunds", "Insufficient pending balance to release.", ErrorType.Validation));
            }
            PendingBalance -= money;
            AvailableBalance += money;
            return Result.Success();
        }

        public Result CancelPendingFunds(Money money)
        {
            if (money.Amount == 0)
            {
                return Result.Failure(new Error("Wallet.CancelPendingFunds", "Amount must be greater than zero.", ErrorType.Validation));
            }
            if (money.Amount > PendingBalance.Amount)
            {
                return Result.Failure(new Error("Wallet.CancelPendingFunds", "Insufficient pending balance to cancel.", ErrorType.Validation));
            }
            PendingBalance -= money;
            return Result.Success();
        }
    }
}
