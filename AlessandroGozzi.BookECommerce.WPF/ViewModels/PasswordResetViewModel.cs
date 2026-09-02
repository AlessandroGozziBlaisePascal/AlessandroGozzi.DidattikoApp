using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.PasswordReset.Action;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.PasswordReset.Request;
using MediatR;

namespace AlessandroGozzi.BookECommerce.WPF.ViewModels
{
    public class PasswordResetViewModel
    {
        private readonly IMediator _mediator;

        public PasswordResetViewModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Step 1: Richiesta invio OTP
        public async Task<bool> RequestOtpAsync(string recapito)
        {
            if (string.IsNullOrWhiteSpace(recapito))
            {
                MessageBox.Show("Inserisci un'email o un numero di telefono valido.", "Campo Obbligatorio", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            var response = await _mediator.Send(new RequestPasswordResetCommand(recapito));

            await Task.Delay(300); // Simulazione
            MessageBox.Show($"Codice OTP inviato a {recapito}. Inseriscilo insieme alla nuova password.", "Codice Inviato", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }

        // Step 2: Invia tutto insieme (OTP + Password)
        public async Task<bool> ExecutePasswordResetAsync(string recapito, string otp, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(otp))
            {
                MessageBox.Show("Inserisci il codice OTP ricevuto.", "Campo Obbligatorio", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Compila entrambi i campi password.", "Campo Obbligatorio", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Le password non coincidono.", "Errore Validatione", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var response = await _mediator.Send(new ResetPasswordCommand(recapito, otp, newPassword, confirmPassword));

            await Task.Delay(500); // Simulazione
            MessageBox.Show("Password aggiornata con successo! Puoi effettuare il login.", "Operazione Completata", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
    }
}
