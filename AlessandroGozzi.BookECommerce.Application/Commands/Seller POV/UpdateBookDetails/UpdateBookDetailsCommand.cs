using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Seller_POV.UpdateBookDetails
{
    public record UpdateBookDetailsCommand(Guid BookId, Guid CustomerId, decimal? NewPrice = null, string? NewStatus = null, string? NewPhoto = null)
    :IRequest<Result<BookDto>>;
}
