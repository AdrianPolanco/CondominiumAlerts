using CondominiumAlerts.Domain.Aggregates.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondominiumAlerts.Infrastructure.Persistence.Context;

public sealed class ApplicationDbContext : DbContext
{
    #region sets

    public DbSet<Condominium> Condominia {  get; set; } 
    public DbSet<CondominiumUser> CondominiumUsers {  get; set; } 
    public DbSet<Post> Posts {  get; set; } 
    public DbSet<Comment> Comments {  get; set; } 
    public DbSet<LevelOfPriority> LevelOfPriorities {  get; set; } 
    public DbSet<Message> Messages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<User> Users { get; set; }

    #endregion

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
    {
        
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(u =>
        {
            u.HasKey(u => u.Id);

            u.HasMany(u => u.MessagesCreatedByUser).WithOne(m => m.CreatorUser).OnDelete(DeleteBehavior.SetNull);

            u.HasMany(u => u.MessagesReceiveByUser).WithOne(m => m.ReceiverUser).OnDelete(DeleteBehavior.SetNull);

            u.HasMany(u => u.NotificacionsReceiveByUser).WithOne(n => n.ReceiverUser).OnDelete(DeleteBehavior.SetNull);

            u.HasMany(u => u.Posts).WithOne(p => p.User).OnDelete(DeleteBehavior.SetNull);

            u.HasMany(u => u.Condominia).WithOne(cu => cu.User).OnDelete(DeleteBehavior.SetNull);
        });
    }
}