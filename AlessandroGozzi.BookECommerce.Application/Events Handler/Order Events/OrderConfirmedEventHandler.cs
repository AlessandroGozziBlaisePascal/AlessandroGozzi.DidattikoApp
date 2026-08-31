using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Orders.Events;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler
{
    public class OrderConfirmedEventHandler: INotificationHandler<OrderConfirmedEvent>
    {
        private readonly ICustomerRepository CustRepo;
        private readonly IOrderRepository OrderRepo;
        private readonly IEmailSender EmailSender;
        private readonly IUnitOfWork UnitOfWork;

        public OrderConfirmedEventHandler(ICustomerRepository custRepo, IOrderRepository orderRepo, IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            CustRepo = custRepo;
            OrderRepo = orderRepo;
            EmailSender = emailSender;
            UnitOfWork = unitOfWork;
        }

        public async Task Handle(OrderConfirmedEvent notification, CancellationToken token)
        {
            var order = await OrderRepo.GetByIdAsync(notification.Id, token);
            if (order == null) return;

            var customer = await CustRepo.GetByIdAsync(order.CustomerId, token);
            if (customer == null) return;

            await EmailSender.SendEmailAsync(customer.Email.Value,"Order confirmation",
                $"Your order got confirmed with success at {notification.OccurredOnUtc}" ,token);

            await UnitOfWork.SaveChangesAsync(token);
        }
    }
}
