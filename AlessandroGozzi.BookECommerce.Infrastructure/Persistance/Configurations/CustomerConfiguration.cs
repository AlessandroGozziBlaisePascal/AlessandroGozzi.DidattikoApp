using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.PasswordHash)
                .IsRequired();

            builder.OwnsOne(c => c.FullName, fullNameBuilder =>
            {
                fullNameBuilder.OwnsOne(fn => fn.Name, nameBuilder =>
                {
                    nameBuilder.Property(n => n.Value)
                        .HasColumnName("FirstName")
                        .HasMaxLength(100)
                        .IsRequired();
                });

                fullNameBuilder.OwnsOne(fn => fn.Surname, surnameBuilder =>
                {
                    surnameBuilder.Property(s => s.Value)
                        .HasColumnName("LastName")
                        .HasMaxLength(100)
                        .IsRequired();
                });
            });

            builder.OwnsOne(c => c.Email, emailBuilder =>
            {
                emailBuilder.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(255)
                    .IsRequired();
            });

            builder.OwnsOne(c => c.Address, addressBuilder =>
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

            builder.OwnsOne(c => c.Number, phoneBuilder =>
            {
                phoneBuilder.Property(p => p.Value)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(50)
                    .IsRequired();
            });

            builder.OwnsOne(c => c.CreditCard, cardBuilder =>
            {
                cardBuilder.WithOwner();

                cardBuilder.Property(cc => cc.Last4Digits)
                           .HasColumnName("CreditCardLast4Digits")
                           .HasMaxLength(4);

                cardBuilder.Ignore(cc => cc.DisplayName);

                cardBuilder.OwnsOne(cc => cc.CardOwner, ownerBuilder =>
                {
                    ownerBuilder.WithOwner();

                    ownerBuilder.OwnsOne(co => co.Name, nBuilder =>
                    {
                        nBuilder.WithOwner();
                        nBuilder.Property(n => n.Value)
                                .HasColumnName("CreditCardOwnerFirstName")
                                .HasMaxLength(100);
                    });

                    ownerBuilder.OwnsOne(co => co.Surname, sBuilder =>
                    {
                        sBuilder.WithOwner();
                        sBuilder.Property(s => s.Value)
                                .HasColumnName("CreditCardOwnerLastName")
                                .HasMaxLength(100);
                    });
                });

                cardBuilder.OwnsOne(cc => cc.ExpiryDate, expiryBuilder =>
                {
                    expiryBuilder.WithOwner();
                    expiryBuilder.Property(e => e.Month).HasColumnName("CreditCardExpiryMonth");
                    expiryBuilder.Property(e => e.Year).HasColumnName("CreditCardExpiryYear");
                });
            });

            builder.Ignore(c => c._domainEvents);
        }
    }
}
