using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CustomerId)
                .IsRequired();

            builder.OwnsMany<CartItem>("Items", itemBuilder =>
            {
                itemBuilder.ToTable("CartItems");
                itemBuilder.WithOwner().HasForeignKey("CartId");

                itemBuilder.HasKey(i => i.Id);

                itemBuilder.Property(i => i.BookId)
                    .IsRequired();

                itemBuilder.Property(i => i.SellerId)
                    .IsRequired();

                itemBuilder.Property(i => i.BookTitle)
                    .HasMaxLength(200)
                    .IsRequired();

                itemBuilder.Property(i => i.Quantity)
                    .IsRequired();

                itemBuilder.OwnsOne(i => i.Price, priceBuilder =>
                {
                    priceBuilder.Property(p => p.Amount)
                        .HasColumnName("Price")
                        .HasPrecision(18, 2)
                        .IsRequired();
                });

                itemBuilder.OwnsOne(i => i.MainPhoto, photoBuilder =>
                {
                    photoBuilder.Property(p => p.Value)
                        .HasColumnName("MainPhotoUrl")
                        .HasMaxLength(500);
                });

            });

            builder.Ignore(c => c.GetItems);
            builder.Ignore(c => c._domainEvents);
        }
    }

}
