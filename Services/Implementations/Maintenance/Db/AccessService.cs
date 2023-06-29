using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Maintenance;
using PeepoGuessrApi.Services.Interfaces.Maintenance.Db;

namespace PeepoGuessrApi.Services.Implementations.Maintenance.Db;

public class AccessService : IAccessService
{
    private readonly IDbContextFactory<MaintenanceDbContext> _maintenanceDbContextFactory;

    public AccessService(IDbContextFactory<MaintenanceDbContext> maintenanceDbContextFactory)
    {
        _maintenanceDbContextFactory = maintenanceDbContextFactory;
    }

    public async Task<Access?> Find()
    {
        await using var context = await _maintenanceDbContextFactory.CreateDbContextAsync();
        return await context.Accesses.FirstOrDefaultAsync();
    }

    public async Task<bool> Update(Access access)
    {
        var updateAccess = new Access
        {
            Id = access.Id,
            Token = access.Token,
            Expire = access.Expire
        };
        await using var context = await _maintenanceDbContextFactory.CreateDbContextAsync();
        context.Update(updateAccess);
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