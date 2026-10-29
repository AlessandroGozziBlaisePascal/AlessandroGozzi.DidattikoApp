using AlessandroGozzi.BookECommerce.Application.Commands.Auth.Login;
using AlessandroGozzi.BookECommerce.MobileGUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AlessandroGozzi.BookECommerce.MobileGUI.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IMediator _mediator;
        private readonly CustomerSession _customerSession;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        public LoginViewModel(IMediator mediator, CustomerSession customerSession)
        {
            _mediator = mediator;
            _customerSession = customerSession;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            await Shell.Current.GoToAsync("//PrincipalPage");
            //try
            //{
            //    var response = await _mediator.Send(new LoginCommand(Username, Password));

            //    if (response.IsFailure)
            //    {
            //        await Shell.Current.DisplayAlert("Errore di Login", response.Error.Description, "OK");
            //        return;
            //    }

            //    _customerSession.StartSession(response.Value);
            //    await Shell.Current.GoToAsync("//PrincipalPage");
            //}
            //finally
            //{
            //    IsBusy = false;
            //}
        }

        [RelayCommand]
        private async Task GoToRegisterAsync() => await Shell.Current.GoToAsync("//RegistrationPage");

        [RelayCommand]
        private async Task GoToForgotPasswordAsync() => await Shell.Current.GoToAsync("//PasswordResetPage");
    }
}
