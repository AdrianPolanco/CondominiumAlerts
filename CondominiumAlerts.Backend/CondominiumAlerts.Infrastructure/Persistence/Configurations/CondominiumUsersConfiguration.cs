using CondominiumAlerts.Domain.Aggregates.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CondominiumAlerts.Infrastructure.Persistence.Configurations
{
    public class CondominiumUsersConfiguration : IEntityTypeConfiguration<CondominiumUser>
    {
        public void Configure(EntityTypeBuilder<CondominiumUser> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.HasOne(c => c.User).WithMany(u => u.Condominiums).HasForeignKey(c => c.UserId);
            builder.HasOne(c => c.Condominium).WithMany(u => u.Users).HasForeignKey(c => c.CondominiumId);

            builder.Property(C => C.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();

            builder.Property(c => c.UserId).IsRequired(false);
        }
    }
}
