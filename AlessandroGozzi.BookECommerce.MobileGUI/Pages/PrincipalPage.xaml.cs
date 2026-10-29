namespace AlessandroGozzi.BookECommerce.MobileGUI.Pages;

public partial class PrincipalPage : ContentPage
{
    public PrincipalPage(ViewModels.PrincipalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}