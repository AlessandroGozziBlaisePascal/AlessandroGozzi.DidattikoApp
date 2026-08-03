using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Access;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Access.Login
{
    public record LoginCommand(
        string Identifier, 
        string Password
    ) : IRequest<Result<LoginResponseDto>>;
}
