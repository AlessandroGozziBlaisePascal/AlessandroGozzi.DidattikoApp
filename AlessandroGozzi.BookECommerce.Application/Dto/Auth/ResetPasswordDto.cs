using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Auth
{
    public record ResetPasswordDto(string ResetTokenCode, string NewPassword, string RepeatedNewPassword)
    {
        public override string ToString() => $"ResetTokenCode: {ResetTokenCode}, NewPassword = [PROTECTED], RepeatedNewPassword = [PROTECTED]}}";
    }
}
