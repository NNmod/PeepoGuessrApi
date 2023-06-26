using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Entities.Databases.Lobby;

namespace PeepoGuessrApi.Databases;

public class LobbyDbContext : DbContext
{
    public DbSet<LobbyType> LobbyTypes { get; set; }
    public DbSet<User> Users { get; set; }
    
    public LobbyDbContext(DbContextOptions<LobbyDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LobbyType>().HasData(
            new LobbyType { Id = 1, Name = "Singleplayer" },
            new LobbyType { Id = 2, Name = "Multiplayer" },
            new LobbyType { Id = 3, Name = "PartyBattle" },
            new LobbyType { Id = 4, Name = "RandomEvents" });
        base.OnModelCreating(modelBuilder);
    }
}