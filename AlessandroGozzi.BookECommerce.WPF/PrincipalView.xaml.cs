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

namespace AlessandroGozzi.BookECommerce.WPF
{
    /// <summary>
    /// Logica di interazione per PrincipalView.xaml
    /// </summary>
    public partial class PrincipalView : Window
    {
        private readonly PrincipalViewModel _viewModel;

        public PrincipalView(PrincipalViewModel viewModel)
        {
            InitializeComponent();
            _viewModel= viewModel;
            DataContext = _viewModel;
        }

        private void BtnCompraLibri_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implementare la logica per la navigazione alla pagina di acquisto libri
        }

        private void BtnVendiLibri_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implementare la logica per la navigazione alla pagina di vendita libri
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
