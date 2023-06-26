using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Game;
using PeepoGuessrApi.Services.Interfaces.Game.Db;

namespace PeepoGuessrApi.Services.Implementations.Game.Db;

public class GameTypeService : IGameTypeService
{
    private readonly IDbContextFactory<GameDbContext> _gameDbContextFactory;

    public GameTypeService(IDbContextFactory<GameDbContext> gameDbContextFactory)
    {
        _gameDbContextFactory = gameDbContextFactory;
    }

    public async Task<GameType?> Find(string name)
    {
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
        return await context.GameTypes
            .FirstOrDefaultAsync(gt => string.Equals(gt.Name.ToLower(), name.ToLower()));
    }

    public async Task<GameType?> FindInclude(string name)
    {
        await using var context = await _gameDbContextFactory.CreateDbContextAsync();
        return await context.GameTypes
            .Include(g => g.Games)
            .ThenInclude(u => u.Users)
            .FirstOrDefaultAsync(gt => string.Equals(gt.Name.ToLower(), name.ToLower()));
    }
}