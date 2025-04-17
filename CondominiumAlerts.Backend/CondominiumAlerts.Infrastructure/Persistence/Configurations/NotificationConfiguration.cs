using CondominiumAlerts.Domain.Aggregates.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CondominiumAlerts.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedOnAdd();
        builder.HasOne(n => n.Condominium)
            .WithMany()
            .HasForeignKey(n => n.CondominiumId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(n => n.LevelOfPriority)
            .WithMany(l => l.Notifications)
            .HasForeignKey(n => n.LevelOfPriorityId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(n => n.Event)
            .WithMany(e => e.Notifications)
            .HasForeignKey(n => n.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
        
        builder.Property(b => b.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
    }
}
