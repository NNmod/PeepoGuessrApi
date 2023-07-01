using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Entities.Databases.Game;

namespace PeepoGuessrApi.Databases;

public class GameDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<GameType> GameTypes { get; set; }
    public DbSet<User> Users { get; set; }
    
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameType>().HasData(
            new GameType { Id = 1, Name = "Singleplayer", RoundDuration = 120, IsPromotionEnable = true, RoundPromotionDuration = 5 },
            new GameType { Id = 2, Name = "Multiplayer", RoundDuration = 180, IsPromotionEnable = true, RoundPromotionDuration = 15 },
            new GameType { Id = 3, Name = "PartyBattle", RoundDuration = 180, IsPromotionEnable = false, RoundPromotionDuration = 0 },
            new GameType { Id = 4, Name = "RandomEvents", RoundDuration = 180, IsPromotionEnable = true, RoundPromotionDuration = 15 });
        base.OnModelCreating(modelBuilder);
    }
}