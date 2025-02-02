using CondominiumAlerts.Domain.Aggregates.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CondominiumAlerts.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasMany(u => u.MessagesCreatedByUser)
            .WithOne(m => m.CreatorUser)
            .HasForeignKey(m => m.CreatorUserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(u => u.MessagesReceivedByUser)
            .WithOne(m => m.ReceiverUser)
            .HasForeignKey(m => m.ReceiverUserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(u => u.NotificationsReceivedByUser)
            .WithOne(n => n.ReceiverUser)
            .HasForeignKey(n => n.ReceiverUserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(u => u.Posts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
        
        builder.Property(b => b.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
        
        // Configuración de Phone como un Value Object dentro de User
        builder.OwnsOne(u => u.Phone, phone =>
        {
            phone.Property(p => p.Number).HasColumnName("PhoneNumber")
                .HasMaxLength(20)
                .IsRequired();
        });
        
        builder.OwnsOne(u => u.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("Street")
                .HasMaxLength(255)
                .IsRequired();

            address.Property(a => a.City).HasColumnName("City")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode).HasColumnName("PostalCode")
                .HasMaxLength(10)
                .IsRequired();;
        });
    }
}