using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.PasswordReset.Action;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.PasswordReset.Request;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AlessandroGozzi.BookECommerce.MobileGUI.ViewModels
{
    public partial class PasswordResetViewModel : ObservableObject
    {
        private readonly IMediator _mediator;

        [ObservableProperty] private string _recapito = string.Empty;
        [ObservableProperty] private string _codiceOtp = string.Empty;
        [ObservableProperty] private string _newPassword = string.Empty;
        [ObservableProperty] private string _confirmPassword = string.Empty;

        // Gestione dello stato di presentazione della UI
        [ObservableProperty] private bool _isPhase2Enabled = false;
        [ObservableProperty] private double _phase2Opacity = 0.45;
        [ObservableProperty] private bool _isBusy;

        public PasswordResetViewModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [RelayCommand]
        private async Task RequestOtpAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var result = await _mediator.Send(new RequestPasswordResetCommand(Recapito));

                if (result.IsFailure)
                {
                    await Shell.Current.DisplayAlert("Richiesta fallita", result.Error.Description, "OK");
                    return;
                }

                IsPhase2Enabled = true;
                Phase2Opacity = 1.0;

                await Shell.Current.DisplayAlert("Codice Inviato", $"Codice inviato a {Recapito}. Procedi con la Fase 2.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ExecutePasswordResetAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var command = new ResetPasswordCommand(Recapito, CodiceOtp, NewPassword, ConfirmPassword);
                var result = await _mediator.Send(command);

                if (result.IsFailure)
                {
                    await Shell.Current.DisplayAlert("Errore Cambio Password", result.Error.Description, "OK");
                    return;
                }

                await Shell.Current.DisplayAlert("Completato", "La password è stata aggiornata con successo.", "OK");
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
