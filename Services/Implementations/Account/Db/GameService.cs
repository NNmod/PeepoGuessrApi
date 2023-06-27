using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Services.Implementations.Account.Db;

public class GameService : IGameService
{
    private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;

    public GameService(IDbContextFactory<AccountDbContext> accountDbContextFactory)
    {
        _accountDbContextFactory = accountDbContextFactory;
    }

    public async Task<Entities.Databases.Account.Game?> Find(int id)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Games
            .Include(gt => gt.GameType)
            .Include(gs => gs.GameStatus)
            .Include(r => r.Rounds)
            .ThenInclude(m => m.Map)
            .Include(s => s.Summaries)
            .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Entities.Databases.Account.Game?> FindByCode(string code)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Games
            .Include(gt => gt.GameType)
            .Include(gs => gs.GameStatus)
            .Include(r => r.Rounds)
            .ThenInclude(m => m.Map)
            .Include(s => s.Summaries)
            .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(g => g.Code == code);
    }

    public async Task<Entities.Databases.Account.Game?> FindByUserAndStatus(int userId, GameStatus gameStatus)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Games
            .Include(s => s.Summaries)
            .FirstOrDefaultAsync(g => g.Summaries.Any(u => u.UserId == userId) && g.GameStatusId == gameStatus.Id);
    }

    public async Task<bool> Create(Entities.Databases.Account.Game game)
    {
        var addGame = new Entities.Databases.Account.Game
        {
            GameTypeId = game.GameTypeId,
            GameStatusId = game.GameStatusId,
            Code = game.Code,
            DateTime = game.DateTime
        };
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        await context.AddAsync(addGame);
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

    public async Task<bool> Update(Entities.Databases.Account.Game game)
    {
        var updateGame = new Entities.Databases.Account.Game
        {
            Id = game.Id,
            GameTypeId = game.GameTypeId,
            GameStatusId = game.GameStatusId,
            Code = game.Code,
            DateTime = game.DateTime
        };
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        context.Update(updateGame);
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