using CondominiumAlerts.Features.Features.Summary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CondominiumAlerts.Infrastructure.Persistence.Configurations;

public class SummaryConfiguration : IEntityTypeConfiguration<Summary>
{
    public void Configure(EntityTypeBuilder<Summary> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(s => s.User).WithMany(u => u.TriggeredSummaries)
            .HasForeignKey(s => s.TriggeredBy).OnDelete(DeleteBehavior.Restrict);;
        
        builder.HasOne(s => s.Condominium).WithMany(c => c.Summaries)
            .HasForeignKey(s => s.CondominiumId).OnDelete(DeleteBehavior.Restrict);;
        
        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
    }
}