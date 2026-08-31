using AlessandroGozzi.BookECommerce.Application;
using AlessandroGozzi.BookECommerce.Application.Commands.StartCheckoutPayment;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books.ValueObjects;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Carts;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers.ValueObjects;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments.ValueObjects;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Wallets;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using AlessandroGozzi_BookECommerce.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace AlessandroGozzi.BookECommerce.ApplicationTests
{
    public class StartCheckoutPaymentCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock = new();
        private readonly Mock<ICustomerRepository> _customerRepoMock = new();
        private readonly Mock<IBookRepository> _bookRepoMock = new();
        private readonly Mock<IPaymentService> _paymentServiceMock = new();
        private readonly Mock<IWalletRepository> _walletRepoMock = new();

        private readonly StartCheckoutPaymentCommandHandler _handler;

        public StartCheckoutPaymentCommandHandlerTests()
        {
            _handler = new StartCheckoutPaymentCommandHandler(
                _cartRepoMock.Object,
                _walletRepoMock.Object,
                _bookRepoMock.Object,
                _paymentServiceMock.Object,
                _customerRepoMock.Object

            );
        }

        [Fact]
        public async Task Handle_WhenCustomerNotFoundOrCreditCardNull_ShouldReturnFailure()
        {
            var command = new StartCheckoutPaymentCommand(Guid.NewGuid(), default(ShippingType));

            _customerRepoMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Customer");
            result.Error.Description.Should().Be("Customer not found or null credit card");
        }

        [Fact]
        public async Task Handle_WhenCreditCardIsExpired_ShouldReturnFailure()
        {
            var command = new StartCheckoutPaymentCommand(Guid.NewGuid(), default(ShippingType));
            var customer = CreateCustomerWithExpiredCard(command.CustomerId);

            _customerRepoMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Credit card");
            result.Error.Description.Should().Be("Credit card is expired");
        }

        [Fact]
        public async Task Handle_WhenCartNotFound_ShouldReturnNotFoundFailure()
        {
            var command = new StartCheckoutPaymentCommand(Guid.NewGuid(), default(ShippingType));
            var customer = CreateCustomerWithValidCard(command.CustomerId);

            _customerRepoMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _cartRepoMock
                .Setup(x => x.GetByCustomerIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Cart");
            result.Error.Description.Should().Be("Cart not found");
            result.Error.Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_WhenCartIsEmpty_ShouldReturnStatusConflictFailure()
        {
            var command = new StartCheckoutPaymentCommand(Guid.NewGuid(), default(ShippingType));
            var customer = CreateCustomerWithValidCard(command.CustomerId);
            var emptyCart = CreateEmptyCart(command.CustomerId);

            _customerRepoMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _cartRepoMock
                .Setup(x => x.GetByCustomerIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyCart);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Cart");
            result.Error.Description.Should().Be("Cart is empty");
            result.Error.Type.Should().Be(ErrorType.StatusConflict);
        }

        [Fact]
        public async Task Handle_WhenWalletCoversTotal_ShouldReturnWalletCoveredResultWithoutPaymentIntent()
        {
            var command = new StartCheckoutPaymentCommand(Guid.NewGuid(), default(ShippingType));
            var customer = CreateCustomerWithValidCard(command.CustomerId);
            var cart = CreateCartWithItems(command.CustomerId);
            var wallet = CreateWallet(command.CustomerId, balanceAmount: 1000m);

            SetupMocksForValidCart(customer, cart, wallet);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.ClientSecret.Should().Be("WALLET_COVERED");
            result.Value.PaymentIntentId.Should().Be("WALLET_PAYMENT");

            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(
                    It.IsAny<decimal>(),
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<string>()
                ),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_WhenPaymentServiceFails_ShouldReturnPaymentServiceError()
        {
            var command = new StartCheckoutPaymentCommand(Guid.NewGuid(), default(ShippingType));
            var customer = CreateCustomerWithValidCard(command.CustomerId);
            var cart = CreateCartWithItems(command.CustomerId);
            var wallet = CreateWallet(command.CustomerId, balanceAmount: 0m);

            SetupMocksForValidCart(customer, cart, wallet);

            var expectedError = new Error("PaymentService", "Stripe Gateway Error", ErrorType.Failure);

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    It.IsAny<decimal>(),
                    command.CustomerId,
                    cart.Id,
                    It.IsAny<CancellationToken>(),
                    It.IsAny<string>()))
                .ReturnsAsync(Result.Failure<CheckoutPaymentResultDto>(expectedError));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(expectedError);
        }

        [Fact]
        public async Task Handle_WhenPaymentServiceSucceeds_ShouldReturnCheckoutResult()
        {
            var command = new StartCheckoutPaymentCommand(Guid.NewGuid(), default(ShippingType));
            var customer = CreateCustomerWithValidCard(command.CustomerId);
            var cart = CreateCartWithItems(command.CustomerId);
            var wallet = CreateWallet(command.CustomerId, balanceAmount: 0m);

            SetupMocksForValidCart(customer, cart, wallet);

            var expectedDto = new CheckoutPaymentResultDto("secret_123", "pi_123", 5000);

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    It.IsAny<decimal>(),
                    command.CustomerId,
                    cart.Id,
                    It.IsAny<CancellationToken>(),
                    It.IsAny<string>()))
                .ReturnsAsync(Result.Success(expectedDto));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedDto);
        }

        [Fact]
        public async Task Handle_WhenWalletNotFound_ShouldReturnNotFoundFailure()
        {
            var command = new StartCheckoutPaymentCommand(Guid.NewGuid(), default(ShippingType));
            var customer = CreateCustomerWithValidCard(command.CustomerId);
            var cart = CreateCartWithItems(command.CustomerId);

            _customerRepoMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _cartRepoMock
                .Setup(x => x.GetByCustomerIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _bookRepoMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Book>());

            _walletRepoMock
                .Setup(x => x.GetByCustomerId(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Wallet?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Wallet");
            result.Error.Description.Should().Be("Walle tnot found");
            result.Error.Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_WhenAmountToChargeIsFractionalLessThanOneEuro_ShouldAdjustTo100Cents()
        {
            var command = new StartCheckoutPaymentCommand(Guid.NewGuid(), default(ShippingType));
            var customer = CreateCustomerWithValidCard(command.CustomerId);
            var cart = CreateCartWithItems(command.CustomerId);
            var wallet = CreateWallet(command.CustomerId, balanceAmount: 19.49m);

            SetupMocksForValidCart(customer, cart, wallet);

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    It.IsAny<decimal>(),
                    command.CustomerId,
                    cart.Id,
                    It.IsAny<CancellationToken>(),
                    It.IsAny<string>()))
                .ReturnsAsync(Result.Success(new CheckoutPaymentResultDto("secret", "pi", 100)));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(100m, command.CustomerId, cart.Id, It.IsAny<CancellationToken>(), "eur"),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenAmountToChargeIsGreaterThanOneEuro_ShouldConvertTotalToCentsCorrectly()
        {
            var command = new StartCheckoutPaymentCommand(Guid.NewGuid(), default(ShippingType));
            var customer = CreateCustomerWithValidCard(command.CustomerId);
            var cart = CreateCartWithItems(command.CustomerId);
            var wallet = CreateWallet(command.CustomerId, balanceAmount: 0m);

            SetupMocksForValidCart(customer, cart, wallet);

            _paymentServiceMock
                .Setup(x => x.CreatePaymentIntentAsync(
                    It.IsAny<decimal>(),
                    command.CustomerId,
                    cart.Id,
                    It.IsAny<CancellationToken>(),
                    It.IsAny<string>()))
                .ReturnsAsync(Result.Success(new CheckoutPaymentResultDto("secret", "pi", 1999)));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(1999m, command.CustomerId, cart.Id, It.IsAny<CancellationToken>(), "eur"),
                Times.Once
            );
        }

        private static Customer CreateCustomerWithValidCard(Guid customerId)
        {
            var customer = CreateBaseCustomer(customerId);
            var creditCard = CreditCard.Create("Mario", "Rossi", "08/30", "0654", customerId).Value;
            customer.AddCreditCard(creditCard);
            return customer;
        }

        private static Customer CreateCustomerWithExpiredCard(Guid customerId)
        {
            var customer = CreateBaseCustomer(customerId);
            var creditCard = CreditCard.Create("Mario", "Rossi", "01/20", "0654", customerId).Value;
            customer.AddCreditCard(creditCard);
            return customer;
        }

        private static Customer CreateBaseCustomer(Guid customerId)
        {
            var fullName = new FullName(Name.Create("Mario").Value, Surname.Create("Rossi").Value);
            var email = Email.Create("mario.rossi@example.com").Value;
            var address = Address.Create("Via Roma", "10", "Milano", "40100").Value;
            var phone = PhoneNumber.Create("3331234567").Value;

            var customer = Customer.Create(fullName, email, address, phone, "HashedPassword123!").Value;

            var idProperty = typeof(Entity).GetProperty("Id") ?? typeof(Customer).GetProperty("Id");
            idProperty?.SetValue(customer, customerId);

            return customer;
        }
        private static Wallet CreateWallet(Guid customerId, decimal balanceAmount)
        {
            var wallet = new Wallet(customerId);
            if (balanceAmount > 0)
            {
                var moneyResult = balanceAmount.ToMoneyDomain();

                if (moneyResult.IsFailure)
                {
                    throw new InvalidOperationException($"CreateWallet ha fallito la creazione del Money ({balanceAmount}): {moneyResult.Error}");
                }

                wallet.Deposit(moneyResult.Value);
            }
            return wallet;
        }
        private void SetupMocksForValidCart(Customer customer, Cart cart, Wallet wallet)
        {
            var books = cart.GetItems.Select(item =>
            {
                var book = Book.Create(
                    "Titolo Esempio",
                    Guid.NewGuid(),
                    Subject.Create("MondoAltruista").Value,
                    ISBN.Create("9788804668237").Value,
                    3,
                    2024,
                    Money.Create(19.99m).Value,
                    BookStatus.LikeNew,
                    ImageUrl.Create("image_example.png").Value
                ).Value;

                var idProperty = typeof(Entity).GetProperty("Id") ?? typeof(Book).GetProperty("Id");
                idProperty?.SetValue(book, item.BookId);

                return book;
            }).ToList();

            _bookRepoMock
                .Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(books);
        }

        private static Cart CreateEmptyCart(Guid customerId)
        {
            return Cart.Create(customerId).Value;
        }

        private static Cart CreateCartWithItems(Guid customerId)
        {
            var cart = Cart.Create(customerId).Value;

            var bookId = Guid.NewGuid();

            cart.AddItem(
                bookId,
                Guid.NewGuid(),
                "Promessi sposi",
                Money.Create(19.99m).Value,
                ImageUrl.Create("image_example.jpg").Value
            );

            return cart;
        }
    }
}
