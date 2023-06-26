using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Services.Implementations.Account.Db;

public class UserService : IUserService
{
    private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;

    public UserService(IDbContextFactory<AccountDbContext> accountDbContextFactory)
    {
        _accountDbContextFactory = accountDbContextFactory;
    }

    public async Task<int> Count()
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Users.CountAsync();
    }

    public async Task<List<User>> FindList(int take, int skip = 0)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Users
            .OrderByDescending(d => d.DivisionId)
            .Include(d => d.Division)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<User?> Find(int id)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Users
            .Include(d => d.Division)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> FindByTwitch(string twitchId)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Users
            .OrderByDescending(d => d.DivisionId)
            .Include(d => d.Division)
            .FirstOrDefaultAsync(u => u.TwitchId == twitchId);
    }

    public async Task<bool> Create(User user)
    {
        var addUser = new User
        {
            TwitchId = user.TwitchId,
            DivisionId = user.DivisionId,
            Name = user.Name,
            ImageUrl = user.ImageUrl,
            Score = user.Score,
            Wins = user.Wins,
            Update = user.Update
        };
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
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
            TwitchId = user.TwitchId,
            DivisionId = user.DivisionId,
            Name = user.Name,
            ImageUrl = user.ImageUrl,
            Score = user.Score,
            Wins = user.Wins,
            Update = user.Update
        };
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
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
}