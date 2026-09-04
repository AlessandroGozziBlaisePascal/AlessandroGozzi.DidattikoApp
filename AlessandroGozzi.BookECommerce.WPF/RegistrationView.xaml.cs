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
    /// Logica di interazione per RegistrationView.xaml
    /// </summary>
    public partial class RegistrationView : Window
    {
        private readonly RegistrationViewModel _viewModel;

        public RegistrationView(RegistrationViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // Gestione Placeholder per i campi di testo
            SetupPlaceholders();
        }

        private async void BtnRegistrati_Click(object sender, RoutedEventArgs e)
        {
            string nome = TxtNome.Text;
            string cognome = TxtCognome.Text;
            string telefono = TxtTelefono.Text;
            string email = TxtEmail.Text;
            string street = TxtVia.Text;
            string civicNumber = TxtCivico.Text;
            string city = TxtCitta.Text;
            string cap = TxtCap.Text;
            string password = TxtPassword.Text;
            string confermaPassword = TxtConfermaPassword.Text;

            /*bool success = await _viewModel.RegisterUserAsync(
                nome, cognome, telefono, email, street, civicNumber, city, cap, password, confermaPassword);

            if (success)
            {
                TornaAlLogin();
            }
            */
            TornaAlLogin();
        }

        private void TornaAlLogin()
        {
            var appHost = App.AppHost;
            var loginView = appHost.Services.GetRequiredService<LoginView>();
            loginView.Show();
            this.Close();
        }

        #region Gestione Placeholder dinamici
        private void SetupPlaceholders()
        {
            AddPlaceholder(TxtNome, "Nome");
            AddPlaceholder(TxtCognome, "Cognome");
            AddPlaceholder(TxtEmail, "Email");
            AddPlaceholder(TxtVia, "Via");
            AddPlaceholder(TxtTelefono, "Telefono");
            AddPlaceholder(TxtCivico, "Civico");
            AddPlaceholder(TxtCitta, "Città");
            AddPlaceholder(TxtCap, "CAP");
            AddPlaceholder(TxtPassword, "Password");
            AddPlaceholder(TxtConfermaPassword, "Conferma Password");
        }

        private void AddPlaceholder(TextBox textBox, string placeholderText)
        {
            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = "";
                    textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholderText;
                    textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
                }
            };
        }
        #endregion
    }
}
