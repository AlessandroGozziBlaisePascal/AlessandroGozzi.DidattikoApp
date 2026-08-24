using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Wallets;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Wallets.Events;
using AlessandroGozzi_BookECommerce.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.AggregateRoots.Wallets
{
    public class WalletTests
    {
        private readonly Guid _customerId = Guid.NewGuid();

        private Wallet CreateWallet()
        {
            return new Wallet(_customerId);
        }

        // ==========================================
        // CONSTRUCTOR TESTS
        // ==========================================
        [Fact]
        public void Constructor_ShouldInitializeBalancesToZero()
        {
            var wallet = CreateWallet();

            wallet.CustomerId.Should().Be(_customerId);
            wallet.AvailableBalance.Amount.Should().Be(0m);
            wallet.PendingBalance.Amount.Should().Be(0m);
            wallet.TotalBalance.Amount.Should().Be(0m);
        }

        // ==========================================
        // DEPOSIT TESTS
        // ==========================================
        [Fact]
        public void Deposit_WhenAmountIsZero_ShouldReturnFailure()
        {
            var wallet = CreateWallet();
            var zeroMoney = Money.Create(0m).Value;

            var result = wallet.Deposit(zeroMoney);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Wallet.Deposit");
            result.Error.Description.Should().Be("Deposit amount must be greater than zero.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Deposit_WhenAmountIsLessThanMinThreshold_ShouldReturnFailure()
        {
            var wallet = CreateWallet();
            var moneyLessThanThreshold = Money.Create(0.50m).Value;

            var result = wallet.Deposit(moneyLessThanThreshold);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Transition amount");
            result.Error.Description.Should().Be("Transitions threshold is 1 €");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Deposit_WithValidAmount_ShouldIncreaseAvailableBalanceAndRaiseBalanceDepositedEvent()
        {
            var wallet = CreateWallet();
            var depositAmount = Money.Create(50m).Value;

            var result = wallet.Deposit(depositAmount);

            result.IsSuccess.Should().BeTrue();
            wallet.AvailableBalance.Amount.Should().Be(50m);
            wallet.TotalBalance.Amount.Should().Be(50m);
            wallet._domainEvents.Should().ContainSingle(e => e is BalanceDepositedEvent);
        }

        // ==========================================
        // WITHDRAW TESTS
        // ==========================================
        [Fact]
        public void Withdraw_WhenAmountExceedsAvailableBalance_ShouldReturnFailure()
        {
            var wallet = CreateWallet();
            wallet.Deposit(Money.Create(20m).Value);
            var withdrawAmount = Money.Create(50m).Value;

            var result = wallet.Withdraw(withdrawAmount);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Wallet.Withdraw");
            result.Error.Description.Should().Be("Insufficient balance for withdrawal.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Withdraw_WithValidAmount_ShouldDecreaseAvailableBalanceAndRaiseBalanceWithdrawnEvent()
        {
            var wallet = CreateWallet();
            wallet.Deposit(Money.Create(100m).Value);
            wallet._domainEvents.Clear();
            var withdrawAmount = Money.Create(30m).Value;

            var result = wallet.Withdraw(withdrawAmount);

            result.IsSuccess.Should().BeTrue();
            wallet.AvailableBalance.Amount.Should().Be(70m);
            wallet.TotalBalance.Amount.Should().Be(70m);
        }

        // ==========================================
        // ADD PENDING FUNDS TESTS
        // ==========================================
        [Fact]
        public void AddPendingFunds_WhenAmountIsZero_ShouldReturnFailure()
        {
            var wallet = CreateWallet();
            var zeroMoney = Money.Create(0m).Value;

            var result = wallet.AddPendingFunds(zeroMoney);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Wallet.AddPendingBalance");
            result.Error.Description.Should().Be("Amount must be greater than zero.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void AddPendingFunds_WithValidAmount_ShouldIncreasePendingBalanceAndTotalBalance()
        {
            var wallet = CreateWallet();
            var pendingAmount = Money.Create(25m).Value;

            var result = wallet.AddPendingFunds(pendingAmount);

            result.IsSuccess.Should().BeTrue();
            wallet.PendingBalance.Amount.Should().Be(25m);
            wallet.AvailableBalance.Amount.Should().Be(0m);
            wallet.TotalBalance.Amount.Should().Be(25m);
        }

        // ==========================================
        // RELEASE PENDING FUNDS TESTS
        // ==========================================
        [Fact]
        public void ReleasePendingFunds_WhenAmountIsZero_ShouldReturnFailure()
        {
            var wallet = CreateWallet();
            var zeroMoney = Money.Create(0m).Value;

            var result = wallet.ReleasePendingFunds(zeroMoney);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Wallet.ReleasePendingFunds");
            result.Error.Description.Should().Be("Amount must be greater than zero.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void ReleasePendingFunds_WhenAmountExceedsPendingBalance_ShouldReturnFailure()
        {
            var wallet = CreateWallet();
            wallet.AddPendingFunds(Money.Create(20m).Value);

            var result = wallet.ReleasePendingFunds(Money.Create(30m).Value);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Wallet.ReleasePendingFunds");
            result.Error.Description.Should().Be("Insufficient pending balance to release.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void ReleasePendingFunds_WithValidAmount_ShouldMoveFundsFromPendingToAvailable()
        {
            var wallet = CreateWallet();
            wallet.AddPendingFunds(Money.Create(50m).Value);

            var result = wallet.ReleasePendingFunds(Money.Create(20m).Value);

            result.IsSuccess.Should().BeTrue();
            wallet.PendingBalance.Amount.Should().Be(30m);
            wallet.AvailableBalance.Amount.Should().Be(20m);
            wallet.TotalBalance.Amount.Should().Be(50m);
        }

        // ==========================================
        // CANCEL PENDING FUNDS TESTS
        // ==========================================
        [Fact]
        public void CancelPendingFunds_WhenAmountIsZero_ShouldReturnFailure()
        {
            var wallet = CreateWallet();
            var zeroMoney = Money.Create(0m).Value;

            var result = wallet.CancelPendingFunds(zeroMoney);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Wallet.CancelPendingFunds");
            result.Error.Description.Should().Be("Amount must be greater than zero.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void CancelPendingFunds_WhenAmountExceedsPendingBalance_ShouldReturnFailure()
        {
            var wallet = CreateWallet();
            wallet.AddPendingFunds(Money.Create(15m).Value);

            var result = wallet.CancelPendingFunds(Money.Create(20m).Value);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Wallet.CancelPendingFunds");
            result.Error.Description.Should().Be("Insufficient pending balance to cancel.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void CancelPendingFunds_WithValidAmount_ShouldReducePendingBalance()
        {
            var wallet = CreateWallet();
            wallet.AddPendingFunds(Money.Create(40m).Value);

            var result = wallet.CancelPendingFunds(Money.Create(15m).Value);

            result.IsSuccess.Should().BeTrue();
            wallet.PendingBalance.Amount.Should().Be(25m);
            wallet.AvailableBalance.Amount.Should().Be(0m);
            wallet.TotalBalance.Amount.Should().Be(25m);
        }
    }
}
