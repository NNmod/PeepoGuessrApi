using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Services.Implementations.Account.Db;

public class RoleService : IRoleService
{
    private readonly IDbContextFactory<AccountDbContext> _accountDbContextFactory;

    public RoleService(IDbContextFactory<AccountDbContext> accountDbContextFactory)
    {
        _accountDbContextFactory = accountDbContextFactory;
    }

    public async Task<Role?> Find(string name)
    {
        await using var context = await _accountDbContextFactory.CreateDbContextAsync();
        return await context.Roles.FirstOrDefaultAsync(r => string.Equals(r.Name.ToLower(), name.ToLower()));
    }
}