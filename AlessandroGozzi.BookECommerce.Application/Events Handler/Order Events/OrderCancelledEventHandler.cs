using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Orders.Events;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler
{
    public class OrderCancelledEventHandler: INotificationHandler<OrderCanceledEvent>
    {
        private readonly IOrderRepository OrderRepo;
        private readonly IEmailSender EmailSender;
        private readonly IUnitOfWork UnitOfWork;
        private readonly ICustomerRepository CustRepo;

        public OrderCancelledEventHandler(IOrderRepository orderRepo, IEmailSender emailSender, IUnitOfWork unitOfWork, ICustomerRepository custRepo)
        {
            OrderRepo = orderRepo;
            EmailSender = emailSender;
            UnitOfWork = unitOfWork;
            CustRepo = custRepo;
        }

        public async Task Handle(OrderCanceledEvent notification, CancellationToken token)
        {
            var order = await OrderRepo.GetByIdAsync(notification.OrderId, token);
            if (order == null) return;

            var customer = await CustRepo.GetByIdAsync(order.CustomerId, token);
            if(customer == null) return;

            await EmailSender.SendEmailAsync(customer.Email.Value, "Order cancelled", 
                $"Your order got cancelled at {notification.OccurredOnUtc}", token);

            await UnitOfWork.SaveChangesAsync(token);
        }
    }
}
