using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Services.Implementations.Account.Db;

public class GameStatusService : IGameStatusService
{
    private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;

    public GameStatusService(IDbContextFactory<AccountDbContext> accountDbContextFactory)
    {
        _accountDbContextFactory = accountDbContextFactory;
    }

    public async Task<GameStatus?> Find(string name)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.GameStatuses.FirstOrDefaultAsync(gs => string.Equals(gs.Name.ToLower(), name.ToLower()));
    }
}