using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.PasswordReset.Action
{
    public record ResetPasswordCommand(
        string Identifier,
        string Code,
        string NewPassword,
        string ConfirmNewPassword
    ) : IRequest<Result>;
}
