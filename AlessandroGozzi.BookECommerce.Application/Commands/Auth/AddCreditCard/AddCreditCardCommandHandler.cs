using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using AlessandroGozzi_BookECommerce.Domain.ValueObjects;
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
            if(cleanNumber.Length != 16)
            {
                return Result.Failure<CreditCardDto>(new Error("CardNumber", "Card number is not 16 digits", ErrorType.Validation));
            }
            string lastFourDigits = cleanNumber.Substring(cleanNumber.Length - 4);

            var customer = await CustRepo.GetByIdAsync(command.CustomerId, token);
            if(customer == null)
            {
                return Result.Failure<CreditCardDto>(new Error("Customer", "Customer not found", ErrorType.NotFound));
            }

            var cardResult = CreditCard.Create(
                command.CardHolderName,
                command.CardHolderSurname,
                command.ExpiryDate,
                lastFourDigits,
                customer.Id
            );

            if (cardResult.IsFailure)
            {
                return Result.Failure<CreditCardDto>(cardResult.Error);
            }

            if (customer.CreditCard != null)
            {
                return Result.Failure<CreditCardDto>(
                    new Error("CreditCard.Exists", "Il cliente possiede già una carta di credito associata.", ErrorType.Validation));
            }

            customer.AddCreditCard(cardResult.Value);

            await CustRepo.UpdateAsync(customer, token);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success(cardResult.Value.ToDto());
        }
    }
}
