namespace AlessandroGozzi.BookECommerce.MobileGUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Pages.RegistrationPage), typeof(Pages.RegistrationPage));
            Routing.RegisterRoute(nameof(Pages.PasswordResetPage), typeof(Pages.PasswordResetPage));
        }
    }
}
