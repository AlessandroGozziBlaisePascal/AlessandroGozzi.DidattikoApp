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
        public Guid WalletId { get; init; }
        public decimal AvailableBalance { get; private set; }
        public decimal PendingBalance { get; private set; }
        public decimal TotalBalance => AvailableBalance + PendingBalance;
        public Name CustomerName { get; private set; }
        public Surname CustomerSurname { get; private set; }

        private Wallet(Guid walletId, Name customerName, Surname customerSurname)
        {
            WalletId = walletId;
            AvailableBalance = 0; 
            PendingBalance = 0;
            CustomerName = customerName;
            CustomerSurname = customerSurname;
        }

        public static Result<Wallet> Create(Guid walletId, Name customerName, Surname customerSurname)
        {
            if (walletId == Guid.Empty)
            {
                return Result.Failure<Wallet>(new Error("Wallet id","WalletId cannot be empty.", ErrorType.Validation));
            }
            if (customerName == null)
            {
                return Result.Failure<Wallet>(new Error("Customer name","CustomerName cannot be null.",ErrorType.Validation));
            }
            if (customerSurname == null)
            {
                return Result.Failure<Wallet>(new Error("Customer surname", "CustomerSurname cannot be null.", ErrorType.Validation));
            }
            var wallet = new Wallet(walletId, customerName, customerSurname);
            return Result.Success(wallet);
        }

        public Result Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                return Result.Failure(new Error("Wallet.Deposit", "Deposit amount must be greater than zero.", ErrorType.Validation));
            }
            AvailableBalance += amount;
            return Result.Success();
        }

        public Result Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                return Result.Failure(new Error("Wallet.Withdraw", "Withdrawal amount must be greater than zero.", ErrorType.Validation));
            }
            if (amount > AvailableBalance)
            {
                return Result.Failure(new Error("Wallet.Withdraw", "Insufficient balance for withdrawal.", ErrorType.Validation));
            }
            AvailableBalance -= amount;
            return Result.Success();
        }

        public Result AddPendingFunds(decimal amount)
        {
            if (amount <= 0)
            {
                return Result.Failure(new Error("Wallet.AddPendingBalance", "Amount must be greater than zero.", ErrorType.Validation));
            }
            PendingBalance += amount;
            return Result.Success();
        }

        public Result ReleasePendingFunds(decimal amount)
        {
            if (amount <= 0)
            {
                return Result.Failure(new Error("Wallet.ReleasePendingFunds", "Amount must be greater than zero.", ErrorType.Validation));
            }
            if (amount > PendingBalance)
            {
                return Result.Failure(new Error("Wallet.ReleasePendingFunds", "Insufficient pending balance to release.", ErrorType.Validation));
            }
            PendingBalance -= amount;
            AvailableBalance += amount;
            return Result.Success();
        }

        public Result CancelPendingFunds(decimal amount)
        {
            if (amount <= 0)
            {
                return Result.Failure(new Error("Wallet.CancelPendingFunds", "Amount must be greater than zero.", ErrorType.Validation));
            }
            if (amount > PendingBalance)
            {
                return Result.Failure(new Error("Wallet.CancelPendingFunds", "Insufficient pending balance to cancel.", ErrorType.Validation));
            }
            PendingBalance -= amount;
            return Result.Success();
        }
    }
}
