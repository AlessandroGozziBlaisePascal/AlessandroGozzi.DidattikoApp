using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using AlessandroGozzi.BookECommerce.Infrastructure;
using AlessandroGozzi.BookECommerce.Infrastructure.Persistance;
using AlessandroGozzi.BookECommerce.WPF.Services;
using AlessandroGozzi.BookECommerce.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe.BillingPortal;

namespace AlessandroGozzi.BookECommerce.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IHost? AppHost { get; private set; }
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddInfrastructureServices(hostContext.Configuration);
                    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(App).Assembly));

                    services.AddSingleton<CustomerSession>();

                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<PasswordResetViewModel>();
                    services.AddTransient<RegistrationViewModel>();

                    services.AddTransient<LoginView>();
                    services.AddTransient<PasswordResetView>();
                    services.AddTransient<RegistrationView>();
                })
                .Build();

            _serviceProvider = AppHost.Services;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var initialView = AppHost.Services.GetRequiredService<LoginView>();

            this.MainWindow = initialView;
            initialView.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (AppHost != null)
            {
                await AppHost.StopAsync();
                AppHost.Dispose();
            }

            base.OnExit(e);
        }
    }


}