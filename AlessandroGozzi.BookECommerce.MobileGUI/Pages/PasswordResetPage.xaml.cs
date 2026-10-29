namespace AlessandroGozzi.BookECommerce.MobileGUI.Pages;

public partial class PasswordResetPage : ContentPage
{
    public PasswordResetPage(ViewModels.PasswordResetViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}