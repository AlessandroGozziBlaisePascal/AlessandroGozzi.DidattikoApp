using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Auth;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using MediatR;

namespace AlessandroGozzi.BookECommerce.WPF.Services
{
    public class CustomerSession
    {
        private readonly ISender _mediator;
        public CustomerDto? CurrentCustomer { get; private set; }
        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public bool IsLoggedIn => CurrentCustomer != null;

        public CustomerSession(ISender mediator)
        {
            _mediator = mediator;
        }
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

        public void UpdateCustomerCreditCard(CreditCardDto? creditCard)
        {
            if (CurrentCustomer == null) return;

            CurrentCustomer = CurrentCustomer with { CreditCard = creditCard };
        }
    }
}
