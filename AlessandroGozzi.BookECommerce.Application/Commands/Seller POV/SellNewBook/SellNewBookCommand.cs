using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.SellNewBook
{
    public sealed record SellNewBookCommand(
        Guid SellerId,
        string Isbn,
        string Title,
        string Subject,
        int SchoolYear,
        int PublicationYear,
        decimal Price,
        string Condition
    ) : IRequest<Result>;
}
