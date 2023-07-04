using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Entities.Databases.Account;

namespace PeepoGuessrApi.Databases;

public class AccountDbContext : DbContext
{
    public DbSet<Division> Divisions { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GameStatus> GameStatuses { get; set; }
    public DbSet<GameType> GameTypes { get; set; }
    public DbSet<Map> Maps { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Round> Rounds { get; set; }
    public DbSet<RoundSummary> RoundSummaries { get; set; }
    public DbSet<Summary> Summaries { get; set; }
    public DbSet<User> Users { get; set; }
    
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Map>().HasData(
            new Map { Id = 1, Name = "PPL6MP1", Url = "ppl6n1", IsClassic = true },
            new Map { Id = 2, Name = "PPL6MP2", Url = "ppl6n2", IsClassic = true });
        modelBuilder.Entity<GameType>().HasData(
            new GameType { Id = 1, Name = "Singleplayer" },
            new GameType { Id = 2, Name = "Multiplayer" },
            new GameType { Id = 3, Name = "PartyBattle" },
            new GameType { Id = 4, Name = "RandomEvents" });
        modelBuilder.Entity<Division>().HasData(
            new Division { Id = 1, Name = "Novichok", MinScore = 0, MaxScore = 399 },
            new Division { Id = 2, Name = "Smesharik", MinScore = 400, MaxScore = 899 },
            new Division { Id = 3, Name = "Gandon", MinScore = 900, MaxScore = 1500 });
        modelBuilder.Entity<GameStatus>().HasData(
            new GameStatus { Id = 1, Name = "Active" },
            new GameStatus { Id = 2, Name = "Canceled" },
            new GameStatus { Id = 3, Name = "Completed" });
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Tester" },
            new Role { Id = 3, Name = "User" });
        base.OnModelCreating(modelBuilder);
    }
}