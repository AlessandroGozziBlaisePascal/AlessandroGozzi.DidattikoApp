using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.CustomerId)
                .IsRequired();

            builder.Property(o => o.Date)
                .IsRequired();

            builder.Property(o => o.ShippingFee)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(o => o.ShippingType)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.OwnsOne(o => o.TotalPrice, priceBuilder =>
            {
                priceBuilder.Property(p => p.Amount)
                    .HasColumnName("TotalPrice")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            builder.OwnsOne(o => o.PaymentDetails, paymentBuilder =>
            {
                paymentBuilder.Property(p => p.PaymentMethod)
                    .HasConversion<string>()
                    .HasColumnName("PaymentMethod")
                    .HasMaxLength(50)
                    .IsRequired();

                paymentBuilder.Property(p => p.Description)
                    .HasColumnName("PaymentDescription")
                    .HasMaxLength(250);
            });

            builder.OwnsMany<OrderItem>("_items", itemBuider =>
            {
                itemBuider.ToTable("OrderItems");
                itemBuider.WithOwner().HasForeignKey("OrderId");
                itemBuider.HasKey(i => i.Id);

                itemBuider.Property(i => i.BookId).IsRequired();
                itemBuider.Property(i => i.SellerId).IsRequired();
                itemBuider.Property(i => i.BookTitle).HasMaxLength(200).IsRequired();
                itemBuider.Property(i => i.Quantity).IsRequired();

                itemBuider.OwnsOne(i => i.Price, priceBuilder =>
                {
                    priceBuilder.Property(p => p.Amount)
                        .HasColumnName("Price")
                        .HasPrecision(18, 2)
                        .IsRequired();
                });

                itemBuider.OwnsOne(i => i.MainPhoto, photoBuilder =>
                {
                    photoBuilder.Property(p => p.Value)
                        .HasColumnName("MainPhotoUrl")
                        .HasMaxLength(500);
                });

                itemBuider.Ignore(i => i.TotalPrice);
                itemBuider.Ignore("_domainEvents");
            });

            builder.Property<List<Guid>>("_shipmentIds")
                .HasColumnName("ShipmentIds");

            builder.Ignore(o => o.ShipmentIds);
            builder.Ignore(o => o.IsFreeShippingApplied);
            builder.Ignore(o => o.Items);
            builder.Ignore(o => o._domainEvents);

        }
    }

}
