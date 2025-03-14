using CondominiumAlerts.Domain.Aggregates.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CondominiumAlerts.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();
        builder.HasOne(m => m.CreatorUser)
            .WithMany(u => u.MessagesCreatedByUser)
            .HasForeignKey(m => m.CreatorUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.ReceiverUser)
            .WithMany(u => u.MessagesReceivedByUser)
            .HasForeignKey(m => m.ReceiverUserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Property(m => m.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
        
        builder.Property(m => m.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
    }
}