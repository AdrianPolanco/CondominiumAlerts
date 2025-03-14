using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Features.Features.Summary;
using Microsoft.EntityFrameworkCore;

namespace CondominiumAlerts.Infrastructure.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public DbSet<Condominium> Condominiums {  get; set; } 
    public DbSet<Post> Posts {  get; set; } 
    public DbSet<Comment> Comments {  get; set; } 
    public DbSet<LevelOfPriority> LevelOfPriorities {  get; set; } 
    public DbSet<Message> Messages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Summary> Summaries { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}