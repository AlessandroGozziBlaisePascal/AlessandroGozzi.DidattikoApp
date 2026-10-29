using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.Registration;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers.ValueObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Maui.ApplicationModel;

namespace AlessandroGozzi.BookECommerce.MobileGUI.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly IMediator _mediator;

        [ObservableProperty] private string _nome = string.Empty;
        [ObservableProperty] private string _cognome = string.Empty;
        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _telefono = string.Empty;
        [ObservableProperty] private string _street = string.Empty;
        [ObservableProperty] private string _civicNumber = string.Empty;
        [ObservableProperty] private string _city = string.Empty;
        [ObservableProperty] private string _cap = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private string _confermaPassword = string.Empty;
        [ObservableProperty] private bool _isBusy;

        public RegistrationViewModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [RelayCommand]
        private async Task RegisterUserAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var addressDto = new AddressDto(Street, CivicNumber, City, Cap);
                var command = new RegistrationCommand(
                    Nome,
                    Cognome,
                    Email,
                    Telefono,
                    addressDto,
                    Password,
                    ConfermaPassword);

                var result = await _mediator.Send(command);

                if (result.IsFailure)
                {
                    await Shell.Current.DisplayAlert("Registrazione non valida", result.Error.Description, "OK");
                    return;
                }

                await Shell.Current.DisplayAlert("Registrazione Effettuata", "Account creato con successo. Procedi con il login.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task BackToLoginAsync() => await Shell.Current.GoToAsync("//LoginPage");
    }
}
