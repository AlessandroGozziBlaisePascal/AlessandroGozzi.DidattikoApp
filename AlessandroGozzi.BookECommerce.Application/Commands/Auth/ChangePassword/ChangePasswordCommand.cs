using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.ChangePassword
{
    public record ChangePasswordCommand(Guid CustomerId, string CurrentPassword, string NewPassword): IRequest<Result>;
}
