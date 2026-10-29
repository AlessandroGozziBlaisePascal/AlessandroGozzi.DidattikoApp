using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using AlessandroGozzi.BookECommerce.Infrastructure.Persistance;
using AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Repositories;
using AlessandroGozzi.BookECommerce.Infrastructure.Services;
using AlessandroGozzi.BookECommerce.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlessandroGozzi.BookECommerce.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1. DbContext con SQLite
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // 2. Unit of Work & Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IShipmentRepository, ShipmentRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();

            // 3. Servizi di Sicurezza, Token e Notifiche
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISMSSender, SMSSender>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtProvider, JwtProvider>();

            return services;
        }

        // Metodo pulito per inizializzare il DB senza Reflection
        public static void InitializeDatabase(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }
    }

}
