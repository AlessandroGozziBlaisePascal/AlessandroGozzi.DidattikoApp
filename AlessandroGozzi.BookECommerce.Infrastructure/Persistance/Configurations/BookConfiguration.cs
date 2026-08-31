using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(b => b.SellerId)
                .IsRequired();

            builder.Property(b => b.SchoolYear)
                .IsRequired();

            builder.Property(b => b.PublicationYear)
                .IsRequired();

            builder.Property(b => b.AverageRating)
                .IsRequired();

            builder.Property(b => b.RatingsNumber)
                .IsRequired();

            builder.Property(b => b.IsAvailable)
                .IsRequired();

            builder.OwnsOne(b => b.ISBNCode, isbnBuilder =>
            {
                isbnBuilder.Property(i => i.Value)
                    .HasColumnName("ISBN")
                    .HasMaxLength(13)
                    .IsRequired();
            });

            builder.OwnsOne(b => b.Subject, subjectBuilder =>
            {
                subjectBuilder.Property(s => s.Value)
                    .HasColumnName("Subject")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.OwnsOne(b => b.Price, priceBuilder =>
            {
                priceBuilder.Property(p => p.Amount)
                    .HasColumnName("Price")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            builder.Property(b => b.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.OwnsOne(b => b.MainPhoto, photoBuilder =>
            {
                photoBuilder.Property(p => p.Value)
                    .HasColumnName("MainPhotoUrl")
                    .HasMaxLength(500);
            });

            builder.OwnsMany(b => b.Reviews, reviewBuilder =>
            {
                reviewBuilder.ToTable("BookReviews");
                reviewBuilder.WithOwner().HasForeignKey("BookId");
                reviewBuilder.Property<int>("Id"); 
                reviewBuilder.HasKey("Id");

                reviewBuilder.Property(r => r.CustomerId)
                    .IsRequired();

                reviewBuilder.OwnsOne(r => r.CustomerName, nameBuilder =>
                {
                    nameBuilder.OwnsOne(fn => fn.Name, nBuilder =>
                    {
                        nBuilder.Property(n => n.Value)
                            .HasColumnName("CustomerFirstName")
                            .HasMaxLength(100)
                            .IsRequired();
                    });

                    nameBuilder.OwnsOne(fn => fn.Surname, sBuilder =>
                    {
                        sBuilder.Property(s => s.Value)
                            .HasColumnName("CustomerLastName")
                            .HasMaxLength(100)
                            .IsRequired();
                    });
                });

                reviewBuilder.Property(r => r.Rating)
                    .IsRequired();

                reviewBuilder.Property(r => r.CreatedAt)
                    .IsRequired();
            });

            builder.Ignore(b => b._domainEvents);
        }
    }

}
