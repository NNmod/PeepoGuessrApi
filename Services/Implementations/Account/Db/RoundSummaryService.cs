using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Services.Implementations.Account.Db;

public class RoundSummaryService : IRoundSummaryService
{
    private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;

    public RoundSummaryService(IDbContextFactory<AccountDbContext> accountDbContextFactory)
    {
        _accountDbContextFactory = accountDbContextFactory;
    }

    public async Task<bool> Create(RoundSummary roundSummary)
    {
        var addRoundSummary = new RoundSummary
        {
            RoundId = roundSummary.RoundId,
            UserId = roundSummary.UserId,
            Health = roundSummary.Health,
            Damage = roundSummary.Damage,
            Distance = roundSummary.Distance,
            PosX = roundSummary.PosX,
            PosY = roundSummary.PosY
        };
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        await context.AddAsync(addRoundSummary);
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

    public async Task<bool> Update(RoundSummary roundSummary)
    {
        var updateRoundSummary = new RoundSummary
        {
            Id = roundSummary.Id,
            RoundId = roundSummary.RoundId,
            UserId = roundSummary.UserId,
            Health = roundSummary.Health,
            Damage = roundSummary.Damage,
            Distance = roundSummary.Distance,
            PosX = roundSummary.PosX,
            PosY = roundSummary.PosY
        };
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        context.Update(updateRoundSummary);
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