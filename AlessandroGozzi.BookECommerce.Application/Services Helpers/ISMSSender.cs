using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Services_Helpers
{
    public interface ISMSSender
    {
        Task SendSmsAsync(string to, string message, CancellationToken cancellationToken = default);
    }
}
