using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<string> GenerateAndSaveOtpAsync(string identifier, CancellationToken cancellationToken = default)
        {
            // Genera codice numerico casuale a 6 cifre
            var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            // Salva in cache con scadenza 5 minuti
            var cacheKey = GetCacheKey(identifier);
            _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));

            return Task.FromResult(otp);
        }

        public Task<bool> ValidateOtpAsync(string identifier, string otp, CancellationToken cancellationToken = default)
        {
            var cacheKey = GetCacheKey(identifier);

            if (_cache.TryGetValue(cacheKey, out string? storedOtp))
            {
                return Task.FromResult(storedOtp == otp);
            }

            return Task.FromResult(false);
        }

        public Task<bool> InvalidateOtpAsync(string identifier, string otp, CancellationToken cancellationToken = default)
        {
            var cacheKey = GetCacheKey(identifier);
            _cache.Remove(cacheKey);

            return Task.FromResult(true);
        }

        private static string GetCacheKey(string identifier) => $"OTP_{identifier.ToLower().Trim()}";
    }
}
