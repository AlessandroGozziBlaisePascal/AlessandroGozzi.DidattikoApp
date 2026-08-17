using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.AddCreditCard
{
    public class AddCreditCardCommandHandler: IRequestHandler<AddCreditCardCommand, Result<CreditCardDto>>
    {
        private readonly ICustomerRepository CustRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddCreditCardCommandHandler(ICustomerRepository custRepo, IUnitOfWork unitOfWork)
        {
            CustRepo = custRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreditCardDto>> Handle(AddCreditCardCommand command, CancellationToken token)
        {
            string cleanNumber = command.CardNumber.Replace(" ", "");
            if(cleanNumber.Length != 4)
            {
                return Result.Failure<CreditCardDto>(new Error("CardNumber", "Card number is too short", ErrorType.Validation));
            }
            string lastFourDigits = cleanNumber.Substring(cleanNumber.Length - 4);
            string maskedCardNumber = $"**** **** **** {lastFourDigits}";

            var customer = await CustRepo.GetByIdAsync(command.CustomerId, token);
            if(customer == null)
            {
                return Result.Failure<CreditCardDto>(new Error("Customer", "Customer not found", ErrorType.NotFound));
            }

            var cardResult = CreditCard.Create(
                command.CardHolderName,
                command.CardHolderSurname,
                command.ExpiryDate,
                maskedCardNumber
            );

            if (cardResult.IsFailure)
            {
                return Result.Failure<CreditCardDto>(cardResult.Error);
            }

            customer.AddCreditCard(cardResult.Value);

            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success(cardResult.Value.ToDto());
        }
    }
}
