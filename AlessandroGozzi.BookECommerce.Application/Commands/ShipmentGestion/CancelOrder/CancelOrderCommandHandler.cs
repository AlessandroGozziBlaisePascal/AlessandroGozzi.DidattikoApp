using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.ShipmentGestion.CancelOrder
{
    public class CancelOrderCommandHandler: IRequestHandler<CancelOrderCommand, Result>
    {
        private readonly IOrderRepository OrderRepo;
        private readonly ICustomerRepository CustomerRepo;
        private readonly IBookRepository BookRepo;
        private readonly IUnitOfWork UnitOfWork;

        public CancelOrderCommandHandler(IOrderRepository orderRepo, ICustomerRepository customerRepo, IBookRepository bookRepo, IUnitOfWork unitOfWork)
        {
            OrderRepo = orderRepo;
            CustomerRepo = customerRepo;
            BookRepo = bookRepo;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CancelOrderCommand command, CancellationToken token)
        {
            var customer = await CustomerRepo.GetByIdAsync(command.CustomerId, token);
            if(customer == null)
            {
                return Result.Failure(new Error("Customer","Customer not found",ErrorType.NotFound));
            }
            if(customer.Id != command.CustomerId)
            {
                return Result.Failure(new Error("Customer", "You are not the buyer, permission denied", ErrorType.PermissionDenied));
            }

            var order = await OrderRepo.GetByIdAsync(command.OrderId, token);

            if(order == null)
            {
                return Result.Failure(new Error("Order", "Order not found", ErrorType.NotFound));
            }
            var cancelResult = order.CancelOrder();
            if (cancelResult.IsFailure)
            {
                return Result.Failure(new Error("Order", "Cannot cancel the order", ErrorType.Failure));
            }

            var bookIds = order.Items.Select(i => i.BookId).ToList();
            var books = await BookRepo.GetByIdsAsync(bookIds, token);

            foreach(var item in order.Items)
            {
                var book = books.FirstOrDefault(b => b.Id == item.BookId);
                if(book != null)
                    book.RestoreAvailability();
            }

            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
