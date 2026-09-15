using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Configurations;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Carts;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Orders;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Shipments;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Wallets;
using Microsoft.EntityFrameworkCore;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<Wallet> Wallets => Set<Wallet>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerConfiguration).Assembly);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=bookecommerce.db");
            }
        }

    }
}

