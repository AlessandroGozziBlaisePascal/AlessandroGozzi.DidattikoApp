using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Auth;

namespace AlessandroGozzi.BookECommerce.WPF.Services
{
    public class CustomerSession
    {
        public CustomerDto? CurrentCustomer { get; private set; }
        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public bool IsLoggedIn => CurrentCustomer != null;


        public void StartSession(LoginResponseDto loginResponse)
        {
            ArgumentNullException.ThrowIfNull(loginResponse);

            CurrentCustomer = loginResponse.Customer;
            AccessToken = loginResponse.AccessToken;
            RefreshToken = loginResponse.RefreshToken;
            ExpiresAt = loginResponse.ExpiresAt;
        }


        public void ClearSession()
        {
            CurrentCustomer = null;
            AccessToken = null;
            RefreshToken = null;
            ExpiresAt = null;
        }
    }
}
