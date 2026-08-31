using System.Configuration;
using System.Data;
using System.Windows;
using AlessandroGozzi.BookECommerce.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Stripe.BillingPortal;

namespace AlessandroGozzi.BookECommerce.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using(var context = new DesignTimeDbContextFactory().CreateDbContext(new string[0]))
            {
                context.Database.Migrate();
            }
        }
    }

}