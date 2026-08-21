using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.UpdateProfile
{
    public record UpdateProfileCommand(
        Guid CustomerId,
        string? Name = null,
        string? Surname = null,
        string? Email = null,
        AddressDto? Address = null,
        string? Number = null
        ): IRequest<Result>;
    
}
