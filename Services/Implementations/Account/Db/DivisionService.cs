using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Services.Implementations.Account.Db;

public class DivisionService : IDivisionService
{
    private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;

    public DivisionService(IDbContextFactory<AccountDbContext> accountDbContextFactory)
    {
        _accountDbContextFactory = accountDbContextFactory;
    }

    public async Task<List<Division>> FindAll()
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Divisions.ToListAsync();
    }

    public async Task<Division?> Find(string name)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Divisions.FirstOrDefaultAsync(d => string.Equals(d.Name.ToLower(), name.ToLower()));
    }

    public async Task<Division?> FindByScore(int score)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Divisions.FirstOrDefaultAsync(d => score >= d.MinScore && score < d.MaxScore);
    }
}