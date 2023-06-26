using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Services.Implementations.Account.Db;

public class SummaryService : ISummaryService
{
    private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;

    public SummaryService(IDbContextFactory<AccountDbContext> accountDbContextFactory)
    {
        _accountDbContextFactory = accountDbContextFactory;
    }

    public async Task<int> Count(int userId)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Summaries.Where(u => u.UserId == userId)
            .CountAsync();
    }

    public async Task<List<Summary>> FindList(int userId, int take, int skip = 0)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Summaries.Where(u => u.UserId == userId)
            .Include(g => g.Game)
            .ThenInclude(gt => gt!.GameType)
            .Include(g => g.Game)
            .ThenInclude(gs => gs!.GameStatus)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Summary?> Find(int id)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Summaries
            .Include(g => g.Game)
            .ThenInclude(gt => gt!.GameType)
            .Include(g => g.Game)
            .ThenInclude(gs => gs!.GameStatus)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> Create(Summary summary)
    {
        var addSummary = new Summary
        {
            GameId = summary.GameId,
            UserId = summary.UserId,
            DivisionId = summary.DivisionId,
            Score = summary.Score
        };
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        await context.AddAsync(addSummary);
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

    public async Task<bool> Update(Summary summary)
    {
        var updateSummary = new Summary
        {
            Id = summary.Id,
            GameId = summary.GameId,
            UserId = summary.UserId,
            DivisionId = summary.DivisionId,
            Score = summary.Score
        };
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        context.Update(updateSummary);
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