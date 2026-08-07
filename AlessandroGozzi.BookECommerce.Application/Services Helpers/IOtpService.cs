using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Services_Helpers
{
    public interface IOtpService
    {
        Task<string> GenerateAhdSaveOtpAsync(string identifier, CancellationToken cancellationToken = default);
        Task<bool> ValidateOtpAsync(string identifier, string otp, CancellationToken cancellationToken = default);
    }
}
