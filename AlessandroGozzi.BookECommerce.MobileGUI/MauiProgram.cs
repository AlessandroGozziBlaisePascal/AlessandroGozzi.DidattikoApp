using System.Reflection;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.Login;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.Registration;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using AlessandroGozzi.BookECommerce.Infrastructure;
using AlessandroGozzi.BookECommerce.MobileGUI.Pages;
using AlessandroGozzi.BookECommerce.MobileGUI.Services;
using AlessandroGozzi.BookECommerce.MobileGUI.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AlessandroGozzi.BookECommerce.MobileGUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // 1. Configurazione appsettings.json (Build Action impostata su EmbeddedResource)
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.json");
            if (stream != null)
            {
                builder.Configuration.AddJsonStream(stream);
            }

            // 2. Registrazione Infrastructure (tipizzata, pulita e diretta)
            builder.Services.AddInfrastructureServices(builder.Configuration);

            // 3. MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                typeof(MauiProgram).Assembly,
                typeof(RegistrationCommandHandler).Assembly
            ));

            // 4. Sessione & Cache
            builder.Services.AddSingleton<CustomerSession>();
            builder.Services.AddMemoryCache();

            // 5. ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<PasswordResetViewModel>();
            builder.Services.AddTransient<RegistrationViewModel>();
            builder.Services.AddTransient<PrincipalViewModel>();

            // 6. Pages (Viste MAUI)
            builder.Services.AddTransient<IntroPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<PasswordResetPage>();
            builder.Services.AddTransient<RegistrationPage>();
            builder.Services.AddTransient<PrincipalPage>();

            // Rimozione sottolineatura nativa Android per i campi di input
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // 7. Inizializzazione Database SQLite
            DependencyInjection.InitializeDatabase(app.Services);

            return app;
        }
    }
}
