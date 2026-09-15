using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.CustomerId)
                .IsRequired();

            builder.OwnsOne(w => w.AvailableBalance, moneyBuilder =>
            {
                moneyBuilder.Property(m => m.Amount)
                    .HasColumnName("AvailableBalance")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            builder.OwnsOne(w => w.PendingBalance, moneyBuilder =>
            {
                moneyBuilder.Property(m => m.Amount)
                    .HasColumnName("PendingBalance")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            builder.Ignore(w => w.TotalBalance);
            builder.Ignore(w => w._domainEvents);
        }
    }


}
