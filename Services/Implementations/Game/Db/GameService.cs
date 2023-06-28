using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Services.Interfaces.Game.Db;

namespace PeepoGuessrApi.Services.Implementations.Game.Db;

public class GameService : IGameService
{
    private readonly IDbContextFactory<GameDbContext> _gameDbContextFactory;

    public GameService(IDbContextFactory<GameDbContext> gameDbContextFactory)
    {
        _gameDbContextFactory = gameDbContextFactory;
    }

    public async Task<int> Count()
    {
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
        return await context.Games
            .CountAsync();
    }

    public async Task<Entities.Databases.Game.Game?> Find(int gameId)
    {
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
        return await context.Games
            .Include(gt => gt.GameType)
            .Include(u => u.Users)
            .FirstOrDefaultAsync(g => g.GameId == gameId);
    }

    public async Task<Entities.Databases.Game.Game?> FindByCode(string code)
    {
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
        return await context.Games
            .Include(u => u.Users)
            .FirstOrDefaultAsync(g => g.Code == code);
    }

    public async Task<bool> Create(Entities.Databases.Game.Game game)
    {
        var addGame = new Entities.Databases.Game.Game
        {
            GameTypeId = game.GameTypeId,
            GameId = game.GameId,
            Code = game.Code,
            RoundCount = game.RoundCount,
            MapUrl = game.MapUrl,
            Multiplier = game.Multiplier,
            PosX = game.PosX,
            PosY = game.PosY,
            IsRoundPromoted = game.IsRoundPromoted,
            RoundExpire = game.RoundExpire,
            RoundDelayExpire = game.RoundDelayExpire
        };
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
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

    public async Task<bool> Update(Entities.Databases.Game.Game game)
    {
        var updateGame = new Entities.Databases.Game.Game
        {
            Id = game.Id,
            GameTypeId = game.GameTypeId,
            GameId = game.GameId,
            Code = game.Code,
            RoundCount = game.RoundCount,
            MapUrl = game.MapUrl,
            Multiplier = game.Multiplier,
            PosX = game.PosX,
            PosY = game.PosY,
            IsRoundPromoted = game.IsRoundPromoted,
            RoundExpire = game.RoundExpire,
            RoundDelayExpire = game.RoundDelayExpire
        };
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
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

    public async Task<bool> Remove(int id)
    {
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
        var game = await context.Games.FirstOrDefaultAsync(g => g.Id == id);
        if (game == null)
            return false;
        context.Remove(game);
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