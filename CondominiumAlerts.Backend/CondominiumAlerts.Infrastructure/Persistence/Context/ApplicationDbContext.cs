using CondominiumAlerts.Domain.Aggregates.Entities;
using Microsoft.EntityFrameworkCore;

namespace CondominiumAlerts.Infrastructure.Persistence.Context;

public sealed class ApplicationDbContext : DbContext
{
    #region sets

    public DbSet<Condominium> Condominiums {  get; set; } 
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}