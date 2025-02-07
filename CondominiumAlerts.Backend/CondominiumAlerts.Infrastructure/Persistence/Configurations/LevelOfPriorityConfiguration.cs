using CondominiumAlerts.Domain.Aggregates.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CondominiumAlerts.Infrastructure.Persistence.Configurations;

public class LevelOfPriorityConfiguration : IEntityTypeConfiguration<LevelOfPriority>
{
    public void Configure(EntityTypeBuilder<LevelOfPriority> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        builder.HasOne(l => l.Condominium)
            .WithMany(c => c.LevelOfPriorities)
            .HasForeignKey(l => l.CondominiumId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(b => b.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();

        // 🔹 Relación con Posts
        builder.HasMany(l => l.Posts)
            .WithOne(p => p.LevelOfPriority)
            .HasForeignKey(p => p.LevelOfPriorityId)
            .OnDelete(DeleteBehavior.Cascade); // Elimina Posts si se borra LevelOfPriority

        // 🔹 Relación con Notifications
        builder.HasMany(l => l.Notifications)
            .WithOne(n => n.LevelOfPriority)
            .HasForeignKey(n => n.LevelOfPriorityId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
        
        builder.Property(b => b.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
    }
}