using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.AddCreditCard;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.WPF.Services;
using MediatR;

namespace AlessandroGozzi.BookECommerce.WPF.ViewModels
{
    public class PrincipalViewModel
    {
        private readonly IMediator _mediator;
        private readonly CustomerSession _customerSession;

        public CustomerDto? CurrentCustomer => _customerSession.CurrentCustomer;

        public PrincipalViewModel(IMediator mediator, CustomerSession customerSession)
        {
            _mediator = mediator;
            _customerSession = customerSession;
        }

        public async Task<bool> ExecuteAddCreditCardAsync(string cardNumber, string cardHolder, string expiryDate, string cvv)
        {
            var response = await _mediator.Send(new AddCreditCardCommand(CurrentCustomer!.CustomerId, cardNumber, cardHolder, expiryDate, cvv));

            if (response.IsFailure)
            {
                MessageBox.Show(response.Error.Description, "Errore Carta di Credito", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
