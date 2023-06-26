using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Lobby;
using PeepoGuessrApi.Services.Interfaces.LobbyDb;

namespace PeepoGuessrApi.Services.Implementations.LobbyDb;

public class UserService : IUserService
{
    private readonly IDbContextFactory<LobbyDbContext> _lobbyDbContextFactory;

    public UserService(IDbContextFactory<LobbyDbContext> lobbyDbContextFactory)
    {
        _lobbyDbContextFactory = lobbyDbContextFactory;
    }

    public async Task<User?> Find(int userId)
    {
        await using var context = await _lobbyDbContextFactory.CreateDbContextAsync();
        return await context.Users
            .Include(lt => lt.LobbyType)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<bool> Create(User user)
    {
        var addUser = new User
        {
            UserId = user.UserId,
            ConnectionId = user.ConnectionId,
            LobbyTypeId = user.LobbyTypeId,
            Name = user.Name,
            ImageUrl = user.ImageUrl,
            DivisionId = user.DivisionId,
            Score = user.Score,
            IsGameFounded = user.IsGameFounded
        };
        await using var context = await _lobbyDbContextFactory.CreateDbContextAsync();
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
            LobbyTypeId = user.LobbyTypeId,
            Name = user.Name,
            ImageUrl = user.ImageUrl,
            DivisionId = user.DivisionId,
            Score = user.Score,
            IsGameFounded = user.IsGameFounded
        };
        await using var context = await _lobbyDbContextFactory.CreateDbContextAsync();
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
        await using var context = await _lobbyDbContextFactory.CreateDbContextAsync();
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