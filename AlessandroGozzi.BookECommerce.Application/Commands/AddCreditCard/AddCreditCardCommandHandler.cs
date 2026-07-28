using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.AddCreditCard
{
    public class AddCreditCardCommandHandler: IRequestHandler<AddCreditCardCommand, Result>
    {
        private readonly ICustomerRepository CustomerRepo;
        private readonly IUnitOfWork UnitOfWork;

        public AddCreditCardCommandHandler(ICustomerRepository customerRepo, IUnitOfWork unitOfWork)
        {
            CustomerRepo = customerRepo;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddCreditCardCommand command, CancellationToken token)
        {
            var owner = await CustomerRepo.GetByIdAsync(command.CustomerId, token);

            if(owner == null)
            {
                return Result.Failure(new Error("Owner", "Owner not found in DB", ErrorType.NotFound));
            }

            var cardNumber = command.CardNumber;
            if(cardNumber == null)
            {
                return Result.Failure(new Error("Card number", "Card number is null", ErrorType.Validation));
            }
            if (cardNumber?.Length != 16 || !cardNumber.All(char.IsDigit))
            {
                return Result.Failure(new Error("Card number", "Card number must be 16 digits", ErrorType.Validation));
            }

            var last4Digits = cardNumber.Substring(12);

            var card = CreditCard.Create(
                command.CardHolderName,
                command.CardHolderSurname,
                command.ExpiryDate,
                last4Digits
            );

            owner.AddCreditCard(card.Value);

            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success(card);
        }
    }
}
