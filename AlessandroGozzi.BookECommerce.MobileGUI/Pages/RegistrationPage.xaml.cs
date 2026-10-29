namespace AlessandroGozzi.BookECommerce.MobileGUI.Pages;

public partial class RegistrationPage : ContentPage
{
    public RegistrationPage(ViewModels.RegistrationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}