using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.Login;
using AlessandroGozzi.BookECommerce.WPF.Services;
using MediatR;

namespace AlessandroGozzi.BookECommerce.WPF.ViewModels
{
    public class LoginViewModel
    {
        private readonly IMediator _mediator;
        private readonly CustomerSession _customerSession;

        public LoginViewModel(IMediator mediator, CustomerSession customerSession)
        {
            _mediator = mediator;
            _customerSession = customerSession;
        }

        public async Task<bool> ExecuteLoginAsync(string username, string password)
        {
            var response = await _mediator.Send(new LoginCommand(username, password));

            if (response.IsFailure)
            {
                MessageBox.Show(response.Error.Description, "Errore di Login", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            _customerSession.StartSession(response.Value);
            return true;
        }
    }
}
