using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Game;
using PeepoGuessrApi.Services.Interfaces.Game.Db;

namespace PeepoGuessrApi.Services.Implementations.Game.Db;

public class UserService : IUserService
{
    private readonly IDbContextFactory<GameDbContext> _gameDbContextFactory;

    public UserService(IDbContextFactory<GameDbContext> gameDbContextFactory)
    {
        _gameDbContextFactory = gameDbContextFactory;
    }

    public async Task<User?> Find(int userId)
    {
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
        return await context.Users
            .Include(g => g.Game)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<bool> Create(User user)
    {
        var addUser = new User
        {
            UserId = user.UserId,
            ConnectionId = user.ConnectionId,
            GameId = user.GameId,
            Name = user.Name,
            ImageUrl = user.ImageUrl,
            DivisionId = user.DivisionId,
            Health = user.Health,
            GuessAvailable = user.GuessAvailable,
            PosX = user.PosX,
            PosY = user.PosY,
            Distance = user.Distance
        };
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
        await context.AddAsync(addUser);
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(User user)
    {
        var updateUser = new User
        {
            Id = user.Id,
            UserId = user.UserId,
            ConnectionId = user.ConnectionId,
            GameId = user.GameId,
            Name = user.Name,
            ImageUrl = user.ImageUrl,
            DivisionId = user.DivisionId,
            Health = user.Health,
            GuessAvailable = user.GuessAvailable,
            PosX = user.PosX,
            PosY = user.PosY,
            Distance = user.Distance
        };
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
        context.Update(updateUser);
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Remove(int id)
    {
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return false;
        context.Remove(user);
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}