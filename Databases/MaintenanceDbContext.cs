using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Entities.Databases.Maintenance;

namespace PeepoGuessrApi.Databases;

public class MaintenanceDbContext : DbContext
{
    public DbSet<Access> Accesses { get; set; }
    public DbSet<Authorization> Authorizations { get; set; }
    public DbSet<Work> Works { get; set; }

    public MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Access>().HasData(
            new Access { Id = 1 });
        base.OnModelCreating(modelBuilder);
    }
}