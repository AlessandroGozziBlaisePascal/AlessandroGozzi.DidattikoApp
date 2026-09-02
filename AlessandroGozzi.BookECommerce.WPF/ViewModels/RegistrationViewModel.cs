using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.Registration;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using MediatR;

namespace AlessandroGozzi.BookECommerce.WPF.ViewModels
{
    public class RegistrationViewModel
    {
        private readonly IMediator _mediator;

        public RegistrationViewModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> RegisterUserAsync(
            string nome,
            string cognome,
            string telefono,
            string email, 
            string street, 
            string civicNumber, 
            string city, 
            string cap,
            string password,
            string confermaPassword)
        {
            // Validazione dei campi obbligatori
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cognome) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Tutti i campi contrassegnati sono obbligatori.", "Campi Mancanti", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Verifica corrispondenza password
            if (password != confermaPassword)
            {
                MessageBox.Show("Le password inserite non coincidono.", "Errore Password", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                var command = new RegistrationCommand(nome, cognome, email, telefono, new AddressDto(street, civicNumber, city, cap), password, confermaPassword);
                var result = await _mediator.Send(command);

                await Task.Delay(500); // Simulazione chiamata di rete

                MessageBox.Show("Registrazione completata con successo! Ora puoi effettuare il login.", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la registrazione: {ex.Message}", "Errore Server", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
