using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Services.Implementations.Account.Db;

public class MapService : IMapService
{
    private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;

    public MapService(IDbContextFactory<AccountDbContext> accountDbContextFactory)
    {
        _accountDbContextFactory = accountDbContextFactory;
    }

    public async Task<List<Map>> FindAll()
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Maps.ToListAsync();
    }

    public async Task<List<Map>> FindClassic()
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Maps.Where(m => m.IsClassic).ToListAsync();
    }

    public async Task<Map?> Find(string name)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Maps.FirstOrDefaultAsync(m => string.Equals(m.Name.ToLower(), name.ToLower()));
    }
}