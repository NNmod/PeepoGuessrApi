using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Maintenance;
using PeepoGuessrApi.Services.Interfaces.Maintenance;

namespace PeepoGuessrApi.Services.Implementations.Maintenance;

public class AuthorizationService : IAuthorizationService
{
    private readonly IDbContextFactory<MaintenanceDbContext> _maintenanceDbContextFactory;

    public AuthorizationService(IDbContextFactory<MaintenanceDbContext> maintenanceDbContextFactory)
    {
        _maintenanceDbContextFactory = maintenanceDbContextFactory;
    }

    public async Task<Authorization?> Find(string code)
    {
        await using var context = await _maintenanceDbContextFactory.CreateDbContextAsync();
        return await context.Authorizations.FirstOrDefaultAsync(a => a.Code == code);
    }

    public async Task<bool> Create(Authorization authorization)
    {
        var addAuthorization = new Authorization
        {
            Code = authorization.Code
        };
        await using var context = await _maintenanceDbContextFactory.CreateDbContextAsync();
        await context.AddAsync(addAuthorization);
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
        await using var context = await _maintenanceDbContextFactory.CreateDbContextAsync();
        var authorization = await context.Authorizations.FirstOrDefaultAsync(a => a.Id == id);
        if (authorization == null)
            return false;
        context.Remove(authorization);
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