using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Services.Implementations.Account.Db;

public class RoundService : IRoundService
{
    private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;

    public RoundService(IDbContextFactory<AccountDbContext> accountDbContextFactory)
    {
        _accountDbContextFactory = accountDbContextFactory;
    }

    public async Task<Round?> Find(int id)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Rounds
            .Include(m => m.Map)
            .Include(rs => rs.RoundSummaries)
            .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Round?> Find(Entities.Databases.Account.Game game, int count)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Rounds
            .Include(m => m.Map)
            .Include(rs => rs.RoundSummaries)
            .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(r => r.GameId == game.Id && r.Count == count);
    }

    public async Task<bool> Create(Round round)
    {
        var addRound = new Round
        {
            GameId = round.GameId,
            MapId = round.MapId,
            Count = round.Count,
            PosX = round.PosX,
            PosY = round.PosY
        };
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        await context.AddAsync(addRound);
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