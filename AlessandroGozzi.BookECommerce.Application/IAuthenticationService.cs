using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application
{
    public interface IAuthenticationService
    {
        Task<bool> VerifyPassword(Guid customerId, string rawPassword, CancellationToken cancellationToken);
    }
}
