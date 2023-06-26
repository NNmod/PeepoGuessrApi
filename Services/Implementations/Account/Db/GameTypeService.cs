using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Services.Implementations.Account.Db;

public class GameTypeService : IGameTypeService
{
    private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;

    public GameTypeService(IDbContextFactory<AccountDbContext> accountDbContextFactory)
    {
        _accountDbContextFactory = accountDbContextFactory;
    }

    public async Task<GameType?> Find(string name)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.GameTypes.FirstOrDefaultAsync(gt => string.Equals(gt.Name.ToLower(), name.ToLower()));
    }
}