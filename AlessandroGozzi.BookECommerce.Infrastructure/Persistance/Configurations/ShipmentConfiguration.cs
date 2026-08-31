using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable("Shipments");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.VendorId)
                .IsRequired();

            builder.Property(s => s.BuyerId)
                .IsRequired();

            builder.Property(s => s.OrderId)
                .IsRequired();

            builder.Property(s => s.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.ShippingType)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.ShippedAtUtc)
                .IsRequired(false);

            builder.OwnsOne(s => s.SubTotal, priceBuilder =>
            {
                priceBuilder.Property(p => p.Amount)
                    .HasColumnName("SubTotal")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            builder.OwnsOne(s => s.ShippingAddress, addressBuilder =>
            {
                addressBuilder.Property(a => a.Street)
                    .HasColumnName("Street")
                    .HasMaxLength(200)
                    .IsRequired();

                addressBuilder.Property(a => a.CivicNumber)
                    .HasColumnName("CivicNumber")
                    .HasMaxLength(20)
                    .IsRequired();

                addressBuilder.Property(a => a.City)
                    .HasColumnName("City")
                    .HasMaxLength(100)
                    .IsRequired();

                addressBuilder.Property(a => a.CAP)
                    .HasColumnName("CAP")
                    .HasMaxLength(10)
                    .IsRequired();
            });

            builder.OwnsOne(s => s.TrackingInfo, trackingBuilder =>
            {
                trackingBuilder.Property(t => t.Carrier)
                    .HasColumnName("TrackingCarrier")
                    .HasMaxLength(100);

                trackingBuilder.Property(t => t.TrackingCode)
                    .HasColumnName("TrackingCode")
                    .HasMaxLength(100);

                trackingBuilder.Property(t => t.TrackingUrl)
                    .HasColumnName("TrackingUrl")
                    .HasMaxLength(500);
            });

            builder.Ignore(s => s._domainEvents);
        }
    }

}
