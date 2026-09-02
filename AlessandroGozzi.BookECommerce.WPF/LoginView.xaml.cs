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
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.Login;
using AlessandroGozzi.BookECommerce.WPF.ViewModels;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AlessandroGozzi.BookECommerce.WPF
{
    /// <summary>
    /// Logica di interazione per LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _viewModel;

        // Il ViewModel viene iniettato automaticamente dal container di Dependency Injection
        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = IdentifierTextBox.Text;
            string password = PasswordTextBox.Password;

            bool isSuccess = await _viewModel.ExecuteLoginAsync(username, password);
        }

        private void BtnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var appHost = App.AppHost;

            var passwordResetView = appHost.Services.GetRequiredService<PasswordResetView>();

            passwordResetView.Show();
            this.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var appHost = App.AppHost;
            var registrationView = appHost.Services.GetRequiredService<RegistrationView>();

            registrationView.Show();
            this.Close();
        }
    }
}
