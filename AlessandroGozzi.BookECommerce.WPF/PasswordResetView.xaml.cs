using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AlessandroGozzi.BookECommerce.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AlessandroGozzi.BookECommerce.WPF
{
    /// <summary>
    /// Logica di interazione per PasswordResetView.xaml
    /// </summary>
    public partial class PasswordResetView : Window
    {
        private readonly PasswordResetViewModel _viewModel;

        public PasswordResetView(PasswordResetViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        // 1. Invia la richiesta OTP e sblocca la Fase 2
        private async void BtnInviaCodice_Click(object sender, RoutedEventArgs e)
        {
            string recapito = TxtRecapito.Text;

            bool isRequested = await _viewModel.RequestOtpAsync(recapito);

            if (isRequested)
            {
                // Sblocca visivamente il pannello per inserire OTP e Nuova Password
                PannelloFase2.IsEnabled = true;
                PannelloFase2.Opacity = 1.0;
            }
        }

        // 2. Click sul pulsante "Conferma" finale (nella Fase 2)
        private async void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string recapito = TxtRecapito.Text;
            string otp = TxtCodiceOtp.Text;
            string newPassword = TxtNewPassword.Password;
            string confirmPassword = TxtConfirmPassword.Password;

            bool isSuccess = await _viewModel.ExecutePasswordResetAsync(recapito, otp, newPassword, confirmPassword);

            if (isSuccess)
            {
                // Torna al Login
                var appHost = App.AppHost;
                var loginView = appHost.Services.GetRequiredService<LoginView>();
                loginView.Show();
                this.Close();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var appHost = App.AppHost;
            var loginView = appHost.Services.GetRequiredService<LoginView>();
            loginView.Show();
            this.Close();
        }
    }
}
